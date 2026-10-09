#!/usr/bin/env python3
"""Kiem tra ky thuat am thanh Hit Me (dung duoc cho ca file placeholder lan file that).

Chay:  python3 audio_qa.py <thu_muc_chua_wav> [audio_manifest.json]
Do duoc: do dai, dinh (dBFS), RMS (dBFS), vo tieng (clipping), DC offset, im lang dau/cuoi,
va do lien mach cua loop (nhay mau o diem noi so voi nhay mau binh thuong).
KHONG danh gia duoc hay/do - phai nghe tren dien thoai that.
"""
import glob
import json
import os
import sys
import wave

import numpy as np


def read_wav(path):
    with wave.open(path, "rb") as w:
        ch, sw, fs, n = w.getnchannels(), w.getsampwidth(), w.getframerate(), w.getnframes()
        raw = w.readframes(n)
    if sw != 2:
        raise ValueError(f"{path}: chi ho tro 16-bit (gap {8 * sw}-bit)")
    x = np.frombuffer(raw, dtype=np.int16).astype(np.float64) / 32768.0
    return (x.reshape(-1, ch) if ch > 1 else x.reshape(-1, 1)), fs


def db(v):
    return 20 * np.log10(max(v, 1e-9))


def analyse(path, meta):
    x, fs = read_wav(path)
    mono = x.mean(axis=1)
    peak = np.max(np.abs(x))
    rms = np.sqrt(np.mean(mono ** 2))
    clip = int(np.sum(np.abs(x) >= 0.999))
    dc = float(np.mean(mono))
    act = np.where(np.abs(mono) > 0.01)[0]
    lead = (act[0] / fs * 1000) if len(act) else None
    trail = ((len(mono) - act[-1]) / fs * 1000) if len(act) else None
    issues = []
    if clip:
        issues.append(f"vo tieng {clip} mau")
    if abs(dc) > 0.01:
        issues.append(f"DC offset {dc:.3f}")
    if peak < 0.05:
        issues.append("qua nho/im lang")
    kind = (meta or {}).get("kind")
    loop = (meta or {}).get("loop", False)
    seam = None
    if loop:
        d_seam = np.max(np.abs(x[0] - x[-1]))
        d_norm = np.percentile(np.abs(np.diff(x, axis=0)).max(axis=1), 99.5)
        seam = float(d_seam / max(d_norm, 1e-9))
        if seam > 1.0:
            issues.append(f"loop co the nghe tiet 'tach' o diem noi (ti le {seam:.2f})")
    if kind == "sfx" and lead is not None and lead > 30:
        issues.append(f"SFX bi tre dau {lead:.0f} ms")
    return dict(file=os.path.basename(path), sec=round(len(mono) / fs, 2), fs=fs, ch=x.shape[1],
                peak_db=round(db(peak), 1), rms_db=round(db(rms), 1), clip=clip,
                lead_ms=None if lead is None else round(lead), trail_ms=None if trail is None else round(trail),
                seam_ratio=None if seam is None else round(seam, 2), issues=issues)


def main():
    folder = sys.argv[1] if len(sys.argv) > 1 else "."
    mpath = sys.argv[2] if len(sys.argv) > 2 else os.path.join(folder, "audio_manifest.json")
    meta = {}
    if os.path.exists(mpath):
        for a in json.load(open(mpath, encoding="utf-8"))["assets"]:
            meta[a["file"]] = a
    bad = 0
    rows = []
    for p in sorted(glob.glob(os.path.join(folder, "*.wav"))):
        r = analyse(p, meta.get(os.path.basename(p)))
        rows.append(r)
        bad += bool(r["issues"])
    print(f"{'file':28} {'sec':>6} {'ch':>2} {'peak':>6} {'rms':>6} {'lead':>5} {'seam':>5}  issues")
    for r in rows:
        print(f"{r['file']:28} {r['sec']:6} {r['ch']:2} {r['peak_db']:6} {r['rms_db']:6} {str(r['lead_ms']):>5} {str(r['seam_ratio']):>5}  {'; '.join(r['issues']) or 'OK'}")
    missing = [f for f in meta if not os.path.exists(os.path.join(folder, f))]
    for f in missing:
        print("THIEU FILE:", f)
    print(f"\n{len(rows)} file, {bad} file co van de, {len(missing)} file thieu")
    sys.exit(1 if bad or missing else 0)


if __name__ == "__main__":
    main()
