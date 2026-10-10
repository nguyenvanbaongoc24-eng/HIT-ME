-- Migration 003: Tighten RLS & Column-level privileges to prevent client coin/XP manipulation
begin;

-- Revoke all table-level privileges from anon and authenticated
revoke all on public.profiles, public.inventory, public.matches, public.reward_transactions, public.quest_events, public.quests from anon, authenticated;

-- Grant SELECT only to authenticated users (governed by RLS)
grant select on public.profiles, public.inventory, public.reward_transactions, public.quests to authenticated;

-- Grant strictly column-level UPDATE on profiles for cosmetic & name fields only (NO coins, NO xp)
grant update (display_name, avatar, equipped_weapon, updated_at) on public.profiles to authenticated;

-- Full privileges reserved for authoritative service_role
grant all on public.profiles, public.inventory, public.matches, public.reward_transactions, public.quest_events, public.quests to service_role;

commit;
