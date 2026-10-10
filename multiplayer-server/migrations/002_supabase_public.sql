-- HIT ME - Migration 002: Supabase Public Schema with RLS and Auth integration
-- Safe and idempotent: can be executed in Supabase SQL Editor.
begin;

-- 1. Profiles Table
create table if not exists public.profiles (
  user_id uuid primary key references auth.users(id) on delete cascade,
  display_name text not null check(char_length(display_name) between 1 and 24),
  avatar text not null default 'Char01_Player',
  xp bigint not null default 0 check(xp >= 0),
  coins bigint not null default 0 check(coins >= 0),
  equipped_weapon text not null default 'dep-to-ong',
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now()
);

-- 2. Inventory Table
create table if not exists public.inventory (
  user_id uuid references public.profiles(user_id) on delete cascade,
  item_id text not null,
  quantity integer not null default 1 check(quantity > 0),
  primary key(user_id, item_id)
);

-- 3. Matches Table
create table if not exists public.matches (
  id uuid primary key,
  mode text not null default 'classic',
  player_count integer not null check(player_count between 2 and 6),
  started_at timestamptz not null default now(),
  ended_at timestamptz,
  winner uuid references public.profiles(user_id) on delete set null,
  verified boolean not null default false
);

-- 4. Reward Transactions Table
create table if not exists public.reward_transactions (
  match_id uuid references public.matches(id) on delete cascade,
  user_id uuid references public.profiles(user_id) on delete cascade,
  outcome text not null check(outcome in ('win', 'lose', 'draw')),
  coins bigint not null check(coins >= 0),
  xp bigint not null check(xp >= 0),
  private_room boolean not null default false,
  policy_version text not null default 'trial-v1',
  created_at timestamptz not null default now(),
  primary key(match_id, user_id)
);

-- 5. Quest Events Table (Deduplication)
create table if not exists public.quest_events (
  event_id text primary key,
  user_id uuid references public.profiles(user_id) on delete cascade,
  kind text not null,
  created_at timestamptz not null default now()
);

-- 6. Quests Table
create table if not exists public.quests (
  user_id uuid references public.profiles(user_id) on delete cascade,
  period text not null,
  kind text not null,
  progress integer not null default 0 check(progress >= 0),
  state text not null default 'Active' check(state in ('Active', 'Completed', 'Claimed', 'Expired')),
  claimed boolean not null default false,
  primary key(user_id, period, kind)
);

-- Enable Row Level Security (RLS)
alter table public.profiles enable row level security;
alter table public.inventory enable row level security;
alter table public.matches enable row level security;
alter table public.reward_transactions enable row level security;
alter table public.quest_events enable row level security;
alter table public.quests enable row level security;

-- Policies:
-- Users can view their own profile, inventory, rewards, quests
do $$ begin
  if not exists (select 1 from pg_policies where schemaname='public' and tablename='profiles' and policyname='select_own_profile') then
    create policy select_own_profile on public.profiles for select to authenticated using (auth.uid() = user_id);
  end if;
  if not exists (select 1 from pg_policies where schemaname='public' and tablename='profiles' and policyname='update_own_display_name_avatar') then
    create policy update_own_display_name_avatar on public.profiles for update to authenticated using (auth.uid() = user_id) with check (auth.uid() = user_id);
  end if;
  if not exists (select 1 from pg_policies where schemaname='public' and tablename='inventory' and policyname='select_own_inventory') then
    create policy select_own_inventory on public.inventory for select to authenticated using (auth.uid() = user_id);
  end if;
  if not exists (select 1 from pg_policies where schemaname='public' and tablename='reward_transactions' and policyname='select_own_rewards') then
    create policy select_own_rewards on public.reward_transactions for select to authenticated using (auth.uid() = user_id);
  end if;
  if not exists (select 1 from pg_policies where schemaname='public' and tablename='quests' and policyname='select_own_quests') then
    create policy select_own_quests on public.quests for select to authenticated using (auth.uid() = user_id);
  end if;
end $$;

-- Service role has full access
grant all on public.profiles, public.inventory, public.matches, public.reward_transactions, public.quest_events, public.quests to service_role;
grant select on public.profiles, public.inventory, public.reward_transactions, public.quests to authenticated;
grant update (display_name, avatar, equipped_weapon, updated_at) on public.profiles to authenticated;

-- Trigger to automatically create profile on signup
create or replace function public.handle_new_user()
returns trigger as $$
begin
  insert into public.profiles (user_id, display_name, avatar, xp, coins, equipped_weapon)
  values (
    new.id,
    coalesce(new.raw_user_meta_data->>'display_name', 'Player_' || substr(new.id::text, 1, 6)),
    'Char01_Player',
    0,
    0,
    'dep-to-ong'
  )
  on conflict (user_id) do nothing;

  -- Default starter weapons
  insert into public.inventory (user_id, item_id, quantity)
  values
    (new.id, 'dep-to-ong', 1),
    (new.id, 'chao', 1),
    (new.id, 'vot', 1)
  on conflict (user_id, item_id) do nothing;

  return new;
end;
$$ language plpgsql security definer;

drop trigger if exists on_auth_user_created on auth.users;
create trigger on_auth_user_created
  after insert on auth.users
  for each row execute function public.handle_new_user();

commit;
