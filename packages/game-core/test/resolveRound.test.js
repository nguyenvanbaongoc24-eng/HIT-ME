import assert from "node:assert/strict";
import test from "node:test";
import { CONFIG } from "@hit-me/shared-config";
import { clampToArena, distanceToArenaWall, resolveRound } from "../dist/index.js";

const baseConfig = {
  ...CONFIG,
  ARENA_RADIUS: 1000,
  ARENA_SHAPE: "circle",
  PLAYER_RADIUS: 30,
  PROJECTILE_RADIUS: 10,
  MAX_PIERCE_TARGETS: 1
};

function player(id, x, y, hp = 3) {
  return {
    id,
    position: { x, y },
    hp,
    alive: hp > 0
  };
}

function action(playerId, x, y, angle) {
  return {
    playerId,
    position: { x, y },
    angle
  };
}

test("hits one player on a straight throw line", () => {
  const result = resolveRound({
    players: [player("a", 0, 0), player("b", 100, 0)],
    actions: [action("a", 0, 0, 0)],
    config: baseConfig
  });

  assert.deepEqual(result.hits, [{ attackerId: "a", targetId: "b" }]);
  assert.equal(result.hpAfter.b, 2);
  assert.deepEqual(result.eliminated, []);
});

test("does not pierce past the first target when MAX_PIERCE_TARGETS is 1", () => {
  const result = resolveRound({
    players: [player("a", 0, 0), player("b", 100, 0), player("c", 200, 0)],
    actions: [action("a", 0, 0, 0)],
    config: baseConfig
  });

  assert.deepEqual(result.hits, [{ attackerId: "a", targetId: "b" }]);
  assert.equal(result.hpAfter.b, 2);
  assert.equal(result.hpAfter.c, 3);
});

test("allows multiple attackers to hit the same player in one simultaneous round", () => {
  const result = resolveRound({
    players: [player("a", 0, 0), player("b", 0, 100), player("c", 100, 0, 3)],
    actions: [
      action("a", 0, 0, 0),
      action("b", 0, 100, -Math.PI / 4)
    ],
    config: baseConfig
  });

  assert.deepEqual(result.hits, [
    { attackerId: "a", targetId: "c" },
    { attackerId: "b", targetId: "c" }
  ]);
  assert.equal(result.hpAfter.c, 1);
});

test("resolves simultaneous deaths in the same round", () => {
  const result = resolveRound({
    players: [player("a", 0, 0, 1), player("b", 100, 0, 1)],
    actions: [
      action("a", 0, 0, 0),
      action("b", 100, 0, Math.PI)
    ],
    config: baseConfig
  });

  assert.deepEqual(result.hits, [
    { attackerId: "a", targetId: "b" },
    { attackerId: "b", targetId: "a" }
  ]);
  assert.deepEqual(result.eliminated.sort(), ["a", "b"]);
});

test("misses when the throw line does not cross any hitbox", () => {
  const result = resolveRound({
    players: [player("a", 0, 0), player("b", 100, 100)],
    actions: [action("a", 0, 0, 0)],
    config: baseConfig
  });

  assert.deepEqual(result.hits, []);
  assert.equal(result.hpAfter.b, 3);
});

test("ignores targets beyond the arena wall", () => {
  const result = resolveRound({
    players: [player("a", 900, 0), player("b", -900, 0)],
    actions: [action("a", 900, 0, 0)],
    config: baseConfig
  });

  assert.deepEqual(result.hits, []);
});

test("finds ellipse wall distance on horizontal, vertical, and diagonal throws", () => {
  const arena = { shape: "ellipse", a: 1000, b: 1750 };
  assert.ok(Math.abs(distanceToArenaWall({ x: 0, y: 0 }, { x: 1, y: 0 }, arena) - 1000) < 0.001);
  assert.ok(Math.abs(distanceToArenaWall({ x: 0, y: 0 }, { x: 0, y: 1 }, arena) - 1750) < 0.001);
  assert.ok(Math.abs(distanceToArenaWall({ x: 0, y: 0 }, { x: Math.SQRT1_2, y: Math.SQRT1_2 }, arena) - 1227.88) < 0.1);
});

test("clamps positions to the playable ellipse and accepts a throw near its wall", () => {
  const arena = { shape: "ellipse", a: 1000, b: 1750 };
  const clamped = clampToArena({ x: 1300, y: 1800 }, arena, 90);
  assert.ok((clamped.x / 910) ** 2 + (clamped.y / 1660) ** 2 <= 1.000001);
  const result = resolveRound({
    players: [player("a", 850, 0), player("b", 945, 0)],
    actions: [action("a", 850, 0, 0)],
    config: { ...baseConfig, ARENA_SHAPE: "ellipse", ARENA_A: 1000, ARENA_B: 1750, PLAYER_RADIUS: 30, PROJECTILE_RADIUS: 10 }
  });
  assert.deepEqual(result.hits, [{ attackerId: "a", targetId: "b" }]);
});

test("ellipse resolution keeps the first target when opponents align on a ray", () => {
  const result = resolveRound({
    players: [player("a", 0, -900), player("b", 0, -400), player("c", 0, 200)],
    actions: [action("a", 0, -900, Math.PI / 2)],
    config: { ...baseConfig, ARENA_SHAPE: "ellipse", ARENA_A: 1000, ARENA_B: 1750 }
  });
  assert.deepEqual(result.hits, [{ attackerId: "a", targetId: "b" }]);
});
