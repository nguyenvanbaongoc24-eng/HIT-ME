#!/usr/bin/env python3
"""Hit Me - bo am thanh PLACEHOLDER tong hop hoan toan bang code (numpy).

Khong dung mau am thanh (sample) co san => khong vuong ban quyen ben thu ba.
Chat luong chi o muc tam de ghep va thu game; thay bang ban that sau, GIU NGUYEN ten file.

Chay:  python3 generate_audio.py [thu_muc_dau_ra]
"""
import json
import os
import sys
import wave

import numpy as np

FS = 44100
rng = np.random.default_rng(7)  # co dinh hat giong => ket qua lap lai duoc
OUT = sys.argv[1] if len(sys.argv) > 1 else "audio_out"
os.makedirs(OUT, exist_ok=True)


# ----------------------------------------------------------------- tien ich
def t_arr(dur):
    return np.arange(int(dur * FS)) / FS


def midi_hz(m):
    return 440.0 * 2 ** ((m - 69) / 12)


def noise(dur):
    return rng.standard_normal(int(dur * FS))


def fft_filter(x, lo=None, hi=None, order=4):
    X = np.fft.rfft(x)
    f = np.fft.rfftfreq(len(x), 1 / FS)
    g = np.ones_like(f)
    if lo:
        g = g / np.sqrt(1 + (lo / np.maximum(f, 1e-9)) ** (2 * order))
    if hi:
        g = g / np.sqrt(1 + (f / hi) ** (2 * order))
    return np.fft.irfft(X * g, n=len(x))


def glide(f0, f1, dur, curve=1.0):
    t = t_arr(dur)
    f = f0 * (f1 / f0) ** ((t / dur) ** curve)
    return np.sin(2 * np.pi * np.cumsum(f) / FS)


def swept_lowpass(x, fc):
    a = 1 - np.exp(-2 * np.pi * fc / FS)
    y = np.empty_like(x)
    s = 0.0
    for i in range(len(x)):
        s += a[i] * (x[i] - s)
        y[i] = s
    return y


def bell(f, dur, amp=1.0, tau=0.25, idx=2.0):
    t = t_arr(dur)
    mod = idx * np.exp(-t / 0.08) * np.sin(2 * np.pi * f * 2.0 * t)
    return amp * np.sin(2 * np.pi * f * t + mod) * np.exp(-t / tau) * np.minimum(1, t / 0.002)


def saw_like(f, dur, harm=8, vib=0.0):
    t = t_arr(dur)
    fr = f * (1 + vib * np.sin(2 * np.pi * 5.5 * t))
    ph = 2 * np.pi * np.cumsum(fr) / FS
    return sum(np.sin(k * ph) / k for k in range(1, harm + 1))


def mix_parts(parts, total):
    n = int(total * FS)
    y = np.zeros(n)
    for x, start, gain in parts:
        i = int(start * FS)
        j = min(n, i + len(x))
        if j > i:
            y[i:j] += x[: j - i] * gain
    return y


def finish(x, peak=0.7, fade_ms=4):
    x = x - np.mean(x)
    m = np.max(np.abs(x))
    if m > 0:
        x = x * (peak / m)
    n = int(fade_ms / 1000 * FS)
    x[:n] *= np.linspace(0, 1, n)
    x[-n:] *= np.linspace(1, 0, n)
    return x


def write_wav(name, x):
    x = np.clip(x, -1, 1)
    stereo = x.ndim == 2
    data = (x * 32767).astype(np.int16)
    with wave.open(os.path.join(OUT, name), "wb") as w:
        w.setnchannels(2 if stereo else 1)
        w.setsampwidth(2)
        w.setframerate(FS)
        w.writeframes(data.tobytes())


# --------------------------------------------------------------------- SFX
def sfx_tap_place():
    t = t_arr(0.14)
    y = glide(560, 330, 0.14) * np.exp(-t / 0.04)
    y += 0.25 * fft_filter(noise(0.14), lo=1500, hi=6000) * np.exp(-t / 0.004)
    return finish(y)


def sfx_countdown_tick():
    t = t_arr(0.09)
    y = (np.sin(2 * np.pi * 1300 * t) + 0.4 * np.sin(2 * np.pi * 2600 * t)) * np.exp(-t / 0.018)
    return finish(y, 0.6)


def sfx_countdown_last():
    t = t_arr(0.2)
    y = np.sin(2 * np.pi * 1760 * t) * np.exp(-t / 0.06) + 0.3 * np.sin(2 * np.pi * 3520 * t) * np.exp(-t / 0.04)
    return finish(y, 0.7)


def sfx_ready():
    return finish(mix_parts([(bell(523.25, 0.7, tau=0.3), 0.0, 0.8), (bell(783.99, 0.7, tau=0.35), 0.09, 1.0)], 0.8))


def sfx_reveal():
    d = 0.4
    t = t_arr(d)
    s = glide(260, 900, d, 0.8)
    n = fft_filter(swept_lowpass(noise(d), 300 * (4000 / 300) ** (t / d)), lo=200)
    env = np.minimum(1, t / 0.02) * np.exp(-t / 0.16)
    return finish((0.6 * s + 0.5 * n) * env)


def sfx_throw():
    d = 0.42
    t = t_arr(d)
    y = swept_lowpass(noise(d), 350 * (5000 / 350) ** (t / d))
    y = fft_filter(y, lo=180)
    return finish(y * np.sin(np.pi * np.clip(t / d, 0, 1)) ** 1.4)


def sfx_hit_bop():
    d = 0.35
    t = t_arr(d)
    thump = glide(200, 55, d, 0.5) * np.exp(-t / 0.09)
    click = fft_filter(noise(d), lo=500, hi=4000) * np.exp(-t / 0.01)
    pop = glide(700, 300, d, 0.4) * np.exp(-t / 0.03)
    return finish(1.0 * thump + 0.5 * click + 0.4 * pop, 0.85)


def sfx_wall_thud():
    d = 0.25
    t = t_arr(d)
    y = glide(140, 70, d, 0.5) * np.exp(-t / 0.05) + 0.35 * fft_filter(noise(d), lo=800, hi=3500) * np.exp(-t / 0.008)
    return finish(y, 0.55)


def sfx_heart_break():
    d = 0.32
    t = t_arr(d)
    crack = fft_filter(noise(d), lo=2000) * np.exp(-t / 0.05)
    fall = glide(1800, 300, d, 0.6) * np.exp(-t / 0.05)
    thump = glide(120, 60, d, 0.5) * np.exp(-t / 0.06)
    return finish(0.7 * crack + 0.5 * fall + 0.5 * thump)


def sfx_eliminate():
    d = 0.9
    t = t_arr(d)
    f = 700 * (110 / 700) ** (t / d) * (1 + 0.03 * np.sin(2 * np.pi * 7 * t))
    ph = 2 * np.pi * np.cumsum(f) / FS
    y = sum(np.sin(k * ph) / k for k in (1, 2, 3, 4, 5))
    y = fft_filter(y, hi=2500)
    return finish(y * np.minimum(1, t / 0.01) * (1 - t / d) ** 0.7, 0.7)


def sfx_win():
    parts = []
    for i, m in enumerate((72, 76, 79, 84)):
        d = 0.35
        n = saw_like(midi_hz(m), d, 8)
        n = fft_filter(n, hi=3500) * np.minimum(1, t_arr(d) / 0.01) * np.exp(-t_arr(d) / 0.3)
        parts.append((n, i * 0.12, 0.6))
    chord = sum(saw_like(midi_hz(m), 1.0, 8, vib=0.004) for m in (72, 76, 79, 84))
    t = t_arr(1.0)
    chord = fft_filter(chord, hi=3500) * np.minimum(1, t / 0.01) * np.exp(-t / 0.55)
    parts.append((chord, 0.5, 0.35))
    for i, f in enumerate((1568, 2093, 2637)):
        parts.append((bell(f, 0.5, tau=0.2), 0.5 + i * 0.05, 0.25))
    return finish(mix_parts(parts, 1.6), 0.7)


def sfx_lose():
    parts = []
    for i, (m, d) in enumerate(((67, 0.28), (64, 0.28), (60, 0.7))):
        t = t_arr(d)
        f = midi_hz(m) * np.where(t > d * 0.55, 1 - 0.06 * (t - d * 0.55) / (d * 0.45), 1.0)
        ph = 2 * np.pi * np.cumsum(f) / FS
        n = np.sin(ph) + 0.4 * np.sin(2 * ph) + 0.2 * np.sin(3 * ph)
        n = n * np.minimum(1, t / 0.01) * np.minimum(1, (d - t) / 0.05)
        parts.append((n, (0.0, 0.3, 0.6)[i], 0.7))
    return finish(mix_parts(parts, 1.4), 0.4)


def sfx_coin():
    return finish(mix_parts([(bell(987.77, 0.5, tau=0.12, idx=1.5), 0.0, 0.8), (bell(1318.5, 0.6, tau=0.2, idx=1.5), 0.07, 1.0)], 0.7), 0.6)


def sfx_purchase():
    parts = [(fft_filter(noise(0.1), lo=2500) * np.exp(-t_arr(0.1) / 0.015), 0.0, 0.5)]
    for i, f in enumerate((1046.5, 1318.5, 1568, 2093)):
        parts.append((bell(f, 0.6, tau=0.25, idx=1.5), 0.05 + i * 0.06, 0.6))
    return finish(mix_parts(parts, 0.9), 0.65)


def sfx_ui_click():
    t = t_arr(0.06)
    y = np.sin(2 * np.pi * 900 * t) * np.exp(-t / 0.012) + 0.3 * fft_filter(noise(0.06), lo=2000) * np.exp(-t / 0.004)
    return finish(y, 0.5)


def sfx_chat_pop():
    t = t_arr(0.12)
    return finish(glide(500, 1400, 0.12, 0.6) * np.exp(-t / 0.03), 0.5)


def sfx_error():
    parts = []
    for s in (0.0, 0.17):
        d = 0.12
        t = t_arr(d)
        sq = sum(np.sin(2 * np.pi * 180 * k * t) / k for k in (1, 3, 5, 7))
        parts.append((fft_filter(sq, hi=1200) * np.minimum(1, t / 0.005) * np.minimum(1, (d - t) / 0.02), s, 1.0))
    return finish(mix_parts(parts, 0.32), 0.3)


# ------------------------------------------------------------ nhac cu (synth)
_cache = {}


def pluck(f, dur, bright=0.8):
    key = ("pluck", round(f, 2), round(dur, 3))
    if key in _cache:
        return _cache[key]
    t = t_arr(dur)
    y = np.zeros_like(t)
    base_tau = 0.5 + 0.6 * min(1.0, 440.0 / f)
    for k in range(1, 9):
        if f * k > 9000:
            break
        y += (1 / k ** 1.1) * (bright ** (k - 1)) * np.sin(2 * np.pi * k * f * t) * np.exp(-t / (base_tau / (1 + 0.9 * (k - 1))))
    y += 0.08 * fft_filter(noise(dur), lo=800, hi=5000) * np.exp(-t / 0.008)
    y *= np.minimum(1, t / 0.002) * np.minimum(1, (dur - t) / 0.03)
    _cache[key] = y * 0.5
    return _cache[key]


def flute(f, dur, vib=0.004):
    t = t_arr(dur)
    vibr = 1 + vib * np.sin(2 * np.pi * 5.2 * t) * np.minimum(1, t / 0.25)
    ph = 2 * np.pi * np.cumsum(f * vibr) / FS
    tone = np.sin(ph) + 0.25 * np.sin(2 * ph) + 0.08 * np.sin(3 * ph)
    breath = 0.05 * fft_filter(noise(dur), lo=2000, hi=6000)
    env = np.minimum(1, t / 0.04) * np.minimum(1, np.maximum(dur - t, 0) / 0.08)
    return (tone + breath) * env * 0.6


def bass(f, dur):
    t = t_arr(dur)
    y = np.sin(2 * np.pi * f * t) + 0.3 * np.sin(4 * np.pi * f * t) + 0.1 * np.sin(6 * np.pi * f * t)
    e = (0.35 + 0.65 * np.exp(-t / 0.12)) * np.minimum(1, t / 0.005) * np.minimum(1, np.maximum(dur - t, 0) / 0.03)
    return y * e * 0.6


def kick():
    t = t_arr(0.25)
    f = 45 + 90 * np.exp(-t / 0.03)
    y = np.sin(2 * np.pi * np.cumsum(f) / FS) * np.exp(-t / 0.09)
    return y + 0.15 * fft_filter(noise(0.25), hi=3000) * np.exp(-t / 0.004)


def snare():
    t = t_arr(0.2)
    return 0.8 * fft_filter(noise(0.2), lo=1500, hi=9000) * np.exp(-t / 0.05) + 0.5 * np.sin(2 * np.pi * 190 * t) * np.exp(-t / 0.05)


def hat(open_=False):
    d = 0.18 if open_ else 0.05
    t = t_arr(d)
    return fft_filter(noise(d), lo=7000) * np.exp(-t / (0.06 if open_ else 0.012))


def wood():
    t = t_arr(0.1)
    return (np.sin(2 * np.pi * 880 * t) + 0.5 * np.sin(2 * np.pi * 1320 * t)) * np.exp(-t / 0.02)


def reverb(x, rt=0.12, length=0.55, seed=1):
    r = np.random.default_rng(seed)
    n = int(length * FS)
    t = np.arange(n) / FS
    ir = fft_filter(r.standard_normal(n), hi=5000) * np.exp(-t / rt)
    m = 1 << int(np.ceil(np.log2(len(x) + n)))
    return np.fft.irfft(np.fft.rfft(x, m) * np.fft.rfft(ir, m), m)[: len(x)]


def render_loop(bpm, bars, builder, wet=0.18, tail=2.5):
    spb = 60.0 / bpm
    step = spb / 4
    total = bars * 4 * spb
    n = int((total + tail) * FS)
    L = np.zeros(n)
    R = np.zeros(n)

    def add(x, start, gain=1.0, pan=0.0):
        i = int(start * FS)
        if i >= n:
            return
        j = min(n, i + len(x))
        ang = (pan + 1) * np.pi / 4
        L[i:j] += x[: j - i] * gain * np.cos(ang)
        R[i:j] += x[: j - i] * gain * np.sin(ang)

    builder(add, step)
    L = L + wet * reverb(L, seed=1)
    R = R + wet * reverb(R, seed=2)
    N = int(total * FS)
    L2, R2 = L[:N].copy(), R[:N].copy()
    L2[: n - N] += L[N:]  # gap duoi dang chuong ve dau => loop lien mach
    R2[: n - N] += R[N:]
    out = np.stack([fft_filter(L2, lo=30, order=2), fft_filter(R2, lo=30, order=2)], axis=1)
    out *= 0.89 / np.max(np.abs(out))
    return out


PROG = [(48, "C"), (45, "Am"), (41, "F"), (43, "G"), (48, "C"), (45, "Am"), (50, "Dm"), (43, "G")]
ARP = {"C": [60, 64, 67, 72], "Am": [57, 60, 64, 69], "F": [57, 60, 65, 69], "G": [59, 62, 67, 71], "Dm": [57, 62, 65, 69]}

MATCH_MELODY = [
    [(0, 3, 76), (3, 1, 79), (4, 3, 81), (7, 1, 79), (8, 4, 76), (12, 2, 74), (14, 2, 76)],
    [(0, 3, 72), (3, 1, 74), (4, 4, 76), (8, 2, 69), (10, 2, 72), (12, 4, 76)],
    [(0, 3, 81), (3, 1, 79), (4, 3, 76), (7, 1, 74), (8, 4, 72), (12, 2, 74), (14, 2, 76)],
    [(0, 4, 79), (4, 2, 76), (6, 2, 74), (8, 4, 76), (12, 4, 74)],
    [(0, 3, 79), (3, 1, 81), (4, 4, 84), (8, 2, 81), (10, 2, 79), (12, 4, 76)],
    [(0, 3, 81), (3, 1, 79), (4, 2, 76), (6, 2, 79), (8, 4, 81), (12, 4, 79)],
    [(0, 2, 74), (2, 2, 76), (4, 4, 79), (8, 3, 81), (11, 1, 79), (12, 4, 76)],
    [(0, 2, 74), (2, 2, 76), (4, 2, 74), (6, 2, 72), (8, 8, 74)],
]
MENU_MELODY = [
    [(0, 6, 76), (6, 2, 74), (8, 8, 72)],
    [(0, 8, 69), (8, 4, 72), (12, 4, 76)],
    [(0, 6, 81), (6, 2, 79), (8, 8, 76)],
    [(0, 8, 74), (8, 4, 76), (12, 4, 74)],
    [(0, 6, 79), (6, 2, 81), (8, 8, 84)],
    [(0, 8, 81), (8, 8, 79)],
    [(0, 4, 74), (4, 4, 76), (8, 8, 79)],
    [(0, 8, 74), (8, 8, 72)],
]


def build_match(add, step):
    K, S, H, HO, W = kick(), snare(), hat(), hat(True), wood()
    for bar in range(8):
        b0 = bar * 16 * step
        root, ch = PROG[bar]
        for s in (0, 6, 8, 11):
            add(K, b0 + s * step, 0.9, 0.0)
        snares = (4, 12, 13, 14, 15) if bar in (3, 7) else (4, 12)
        for s in snares:
            add(S, b0 + s * step, 0.55 if s < 13 else 0.4, 0.05)
        for s in range(0, 16, 2):
            add(H, b0 + s * step, 0.26 if s % 4 == 0 else 0.18, 0.4)
        add(HO, b0 + 14 * step, 0.2, 0.4)
        for s in (3, 11):
            add(W, b0 + s * step, 0.25, -0.4)
        for s, off in ((0, 0), (3, 0), (6, 7), (8, 0), (11, 12), (14, 7)):
            add(bass(midi_hz(root + off), 0.22), b0 + s * step, 0.5, 0.0)
        for i, s in enumerate(range(0, 16, 2)):
            note = ARP[ch][[0, 1, 2, 3, 2, 1, 2, 3][i]]
            add(pluck(midi_hz(note), 0.9), b0 + s * step, 0.28 if s % 8 == 0 else 0.2, -0.35)
        for s, ln, m in MATCH_MELODY[bar]:
            add(flute(midi_hz(m), ln * step * 0.95), b0 + s * step, 0.5, 0.1)
            if bar >= 4:
                add(pluck(midi_hz(m + 12), 0.5), b0 + s * step, 0.1, 0.3)


def build_menu(add, step):
    K, H, W = kick(), hat(), wood()
    for bar in range(8):
        b0 = bar * 16 * step
        root, ch = PROG[bar]
        if bar % 2 == 0:
            add(K, b0, 0.4, 0.0)
        for s in range(0, 16, 2):
            add(H, b0 + s * step, 0.1, 0.4)
        add(W, b0 + 8 * step, 0.12, -0.4)
        add(bass(midi_hz(root), 8 * step * 0.95), b0, 0.45, 0.0)
        add(bass(midi_hz(root), 8 * step * 0.95), b0 + 8 * step, 0.4, 0.0)
        for i, s in enumerate(range(0, 16, 2)):
            note = ARP[ch][[0, 1, 2, 3, 2, 1, 2, 3][i]]
            add(pluck(midi_hz(note), 1.4), b0 + s * step, 0.3 if s % 8 == 0 else 0.22, -0.3)
        for s, ln, m in MENU_MELODY[bar]:
            add(flute(midi_hz(m), ln * step * 0.95, vib=0.006), b0 + s * step, 0.55, 0.15)


def ambience_crowd(dur=12.0, xfade=2.0):
    total = dur + xfade
    n = int(total * FS)
    chans = []
    for seed in (11, 12):
        r = np.random.default_rng(seed)
        y = np.zeros(n)
        for fc, bw in ((350, 250), (700, 400), (1200, 600), (2000, 900)):
            base = fft_filter(r.standard_normal(n), lo=max(fc - bw, 60), hi=fc + bw)
            slow = fft_filter(r.standard_normal(n), hi=1.2)
            slow = np.clip(0.6 + 1.2 * slow / (np.std(slow) + 1e-9) * 0.5, 0.15, 1.6)
            y += base * slow
        chans.append(y)
    x = np.stack(chans, axis=1)
    keep = int(dur * FS)
    xf = int(xfade * FS)
    out = x[:keep].copy()
    w = np.linspace(0, np.pi / 2, xf)
    out[:xf] = x[:xf] * np.sin(w)[:, None] + x[keep : keep + xf] * np.cos(w)[:, None]
    out *= 0.5 / np.max(np.abs(out))
    return out


# --------------------------------------------------------------------- main
SFX = {
    "sfx_tap_place": (sfx_tap_place, "place_position", "SFX", "Chạm đặt vị trí trên sân"),
    "sfx_countdown_tick": (sfx_countdown_tick, "countdown_tick", "SFX", "Tích mỗi giây (giây 5 đến 3)"),
    "sfx_countdown_last": (sfx_countdown_last, "countdown_last", "SFX", "Tích cao hơn (2 giây cuối)"),
    "sfx_ready": (sfx_ready, "ready_pressed", "UI", "Bấm nút SẴN SÀNG"),
    "sfx_reveal": (sfx_reveal, "round_reveal", "SFX", "Tất cả nhân vật lộ diện"),
    "sfx_throw": (sfx_throw, "weapon_throw", "SFX", "Vũ khí bay (vút)"),
    "sfx_hit_bop": (sfx_hit_bop, "player_hit", "SFX", "Trúng người (BỐP) - có công tắc riêng trong cài đặt"),
    "sfx_wall_thud": (sfx_wall_thud, "weapon_hit_wall", "SFX", "Vũ khí chạm tường"),
    "sfx_heart_break": (sfx_heart_break, "heart_lost", "SFX", "Mất một trái tim"),
    "sfx_eliminate": (sfx_eliminate, "player_eliminated", "SFX", "Người chơi bị loại"),
    "sfx_win": (sfx_win, "match_win", "SFX", "Thắng trận (fanfare ngắn)"),
    "sfx_lose": (sfx_lose, "match_lose", "SFX", "Thua trận"),
    "sfx_coin": (sfx_coin, "gold_gain", "UI", "Nhận vàng"),
    "sfx_purchase": (sfx_purchase, "purchase_success", "UI", "Mua trang bị thành công"),
    "sfx_ui_click": (sfx_ui_click, "ui_click", "UI", "Bấm nút thường"),
    "sfx_chat_pop": (sfx_chat_pop, "chat_message", "UI", "Có tin nhắn chat"),
    "sfx_error": (sfx_error, "ui_error", "UI", "Lỗi, không đủ vàng"),
}


def main():
    manifest = []
    for name, (fn, event, bus, desc) in SFX.items():
        x = fn()
        write_wav(name + ".wav", x)
        manifest.append(dict(id=name, file=name + ".wav", kind="sfx", event=event, bus=bus, loop=False, description=desc, channels=1))
        print("sfx", name, round(len(x) / FS, 2), "s")

    m = render_loop(120, 8, build_match)
    write_wav("bgm_match_loop.wav", m)
    manifest.append(dict(id="bgm_match_loop", file="bgm_match_loop.wav", kind="music", event="music_match", bus="Music", loop=True, bpm=120, bars=8, description="Nhạc nền trong trận, vui nhộn, pentatonic", channels=2))
    print("music match", round(len(m) / FS, 2), "s")

    mm = render_loop(90, 8, build_menu)
    write_wav("bgm_menu_loop.wav", mm)
    manifest.append(dict(id="bgm_menu_loop", file="bgm_menu_loop.wav", kind="music", event="music_menu", bus="Music", loop=True, bpm=90, bars=8, description="Nhạc sảnh/menu, nhẹ nhàng", channels=2))
    print("music menu", round(len(mm) / FS, 2), "s")

    a = ambience_crowd()
    write_wav("amb_crowd_murmur_loop.wav", a)
    manifest.append(dict(id="amb_crowd_murmur_loop", file="amb_crowd_murmur_loop.wav", kind="ambience", event="ambience_crowd", bus="Ambience", loop=True, description="Tiếng khán giả xì xào (tổng hợp từ nhiễu, chỉ là tạm)", channels=2))
    print("ambience", round(len(a) / FS, 2), "s")

    with open(os.path.join(OUT, "audio_manifest.json"), "w", encoding="utf-8") as f:
        json.dump(dict(sampleRate=FS, bitDepth=16, assets=manifest), f, ensure_ascii=False, indent=2)


if __name__ == "__main__":
    main()
