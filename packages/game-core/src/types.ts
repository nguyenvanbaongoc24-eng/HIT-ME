import type { GameConfig } from "@hit-me/shared-config";

export type Vector2 = {
  x: number;
  y: number;
};

export type ArenaBounds =
  | { shape: "circle"; radius: number }
  | { shape: "ellipse"; a: number; b: number };

export type PlayerState = {
  id: string;
  position: Vector2;
  hp: number;
  alive: boolean;
};

export type RoundAction = {
  playerId: string;
  position: Vector2;
  angle: number;
};

export type Hit = {
  attackerId: string;
  targetId: string;
};

export type ResolveRoundInput = {
  players: readonly PlayerState[];
  actions: readonly RoundAction[];
  config: GameConfig;
  arenaRadius?: number;
  arena?: ArenaBounds;
};

export type ResolveRoundResult = {
  hits: Hit[];
  hpAfter: Record<string, number>;
  eliminated: string[];
};
