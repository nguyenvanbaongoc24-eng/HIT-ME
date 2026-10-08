export const THEME = {
  colors: {
    duskTop: 0x3a2438,
    duskBottom: 0x2a1a2b,
    floorLight: 0xf6d29a,
    floorDark: 0xe9b472,
    grout: 0xd79f5f,
    wallTop: 0xc65d3b,
    wallInner: 0x8f3b26,
    wallHighlight: 0xe07b54,
    teal: 0x2fa7a0,
    tealDark: 0x1e7a76,
    gold: 0xf2b544,
    heartFull: 0xef4b5a,
    heartEmpty: 0x6b4a55,
    player: [0x3ba9ff, 0xff5a5f, 0x4cc36b, 0xffc93c, 0xa06cff, 0xff7eb6],
    skin: 0xffd2a3,
    hair: 0x4a2c26,
    text: "#fff6dc",
    paper: 0xffe5b5,
    paperShade: 0xd5935d,
    shadow: 0x56383a,
    hit: 0xfff1a3
  },
  fontFamily: "Nunito, Arial, sans-serif",
  layout: {
    hudTop: 14,
    ticketHeight: 42,
    ticketGap: 5,
    crowdRows: 3,
    crowdMobile: 48,
    particlesPerBurst: 14,
    aimSegments: 16
  },
  motion: {
    hudTickMs: 33,
    crowdBobMs: 560,
    torchFlickerMs: 130,
    hitMs: 120,
    damageFloatMs: 480,
    eliminationMs: 520,
    particleMs: 520
  }
} as const;

export type Quality = "high" | "medium" | "low";
