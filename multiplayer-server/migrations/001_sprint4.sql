-- New isolated schema; does not overwrite existing profiles / inventory tables.
-- Run as database administrator only after reviewing backup and existing schema.
begin;
create schema if not exists hitme_s4;
create table if not exists hitme_s4.profiles (
 user_id uuid primary key references auth.users(id) on delete cascade,
 display_name text not null check(char_length(display_name) between 1 and 24),
 avatar text not null default 'Char01_Player',
 xp bigint not null default 0 check(xp>=0), coins bigint not null default 0 check(coins>=0),
 equipped_weapon text not null default 'dep-to-ong', created_at timestamptz not null default now()
);
create table if not exists hitme_s4.inventory (
 user_id uuid references hitme_s4.profiles(user_id),item_id text not null,quantity integer not null default 1 check(quantity>0),primary key(user_id,item_id)
);
create table if not exists hitme_s4.matches (
 id uuid primary key,mode text not null,player_count integer not null check(player_count between 2 and 6),
 started_at timestamptz not null,ended_at timestamptz,winner uuid,verified boolean not null default false
);
create table if not exists hitme_s4.reward_transactions (
 match_id uuid references hitme_s4.matches(id),user_id uuid references hitme_s4.profiles(user_id),
 outcome text not null check(outcome in ('win','lose','draw')), coins bigint not null check(coins>=0),xp bigint not null check(xp>=0),
 private_room boolean not null,created_at timestamptz not null default now(),primary key(match_id,user_id)
);
create table if not exists hitme_s4.quest_events (
 event_id text primary key,user_id uuid references hitme_s4.profiles(user_id),kind text not null,created_at timestamptz not null default now()
);
create table if not exists hitme_s4.quests (
 user_id uuid references hitme_s4.profiles(user_id),period text not null,kind text not null,
 progress integer not null default 0, state text not null default 'Active' check(state in ('Active','Completed','Claimed','Expired')),
 primary key(user_id,period,kind)
);
alter table hitme_s4.profiles enable row level security;
alter table hitme_s4.inventory enable row level security;
alter table hitme_s4.matches enable row level security;
alter table hitme_s4.reward_transactions enable row level security;
alter table hitme_s4.quest_events enable row level security;
alter table hitme_s4.quests enable row level security;
revoke all on schema hitme_s4 from anon;
grant usage on schema hitme_s4 to authenticated,service_role;
revoke all on all tables in schema hitme_s4 from anon,authenticated;
grant select on hitme_s4.profiles,hitme_s4.inventory,hitme_s4.reward_transactions,hitme_s4.quests to authenticated;
grant all on all tables in schema hitme_s4 to service_role;
-- Idempotent policy creation; no client INSERT/UPDATE/DELETE permissions.
do $$ begin
 if not exists(select 1 from pg_policies where schemaname='hitme_s4' and tablename='profiles' and policyname='own_profile') then
  create policy own_profile on hitme_s4.profiles for select to authenticated using(user_id=auth.uid());
 end if;
 if not exists(select 1 from pg_policies where schemaname='hitme_s4' and tablename='inventory' and policyname='own_inventory') then
  create policy own_inventory on hitme_s4.inventory for select to authenticated using(user_id=auth.uid());
 end if;
 if not exists(select 1 from pg_policies where schemaname='hitme_s4' and tablename='reward_transactions' and policyname='own_rewards') then
  create policy own_rewards on hitme_s4.reward_transactions for select to authenticated using(user_id=auth.uid());
 end if;
 if not exists(select 1 from pg_policies where schemaname='hitme_s4' and tablename='quests' and policyname='own_quests') then
  create policy own_quests on hitme_s4.quests for select to authenticated using(user_id=auth.uid());
 end if;
end $$;
commit;
