#!/usr/bin/env python3
from PIL import Image, ImageSequence
from collections import defaultdict
import sys, os

PALETTE = [
    (0, 0, 0), (64, 64, 64), (96, 96, 96), (255, 255, 255),
    (32, 0, 0), (64, 0, 0), (96, 0, 0), (255, 0, 0),
    (0, 32, 0), (0, 64, 0), (0, 96, 0), (0, 255, 0),
    (0, 0, 32), (0, 0, 64), (0, 0, 96), (0, 0, 255),
    (32, 0, 32), (64, 0, 64), (96, 0, 96), (255, 0, 255),
    (32, 32, 0), (64, 64, 0), (96, 96, 0), (255, 255, 0),
    (0, 64, 64), (0, 64, 64), (0, 96, 96), (0, 255, 255),
    (0, 0, 0), (64, 64, 64), (96, 96, 96), (255, 255, 255)
]

WINW, WINH = 128, 96

def nearest_palette_index(rgb):
    r,g,b = rgb
    best_i = 0
    best_d = 10**9
    for i,(pr,pg,pb) in enumerate(PALETTE):
        d = (r-pr)**2 + (g-pg)**2 + (b-pb)**2
        if d < best_d:
            best_d = d
            best_i = i
    return best_i & 0x1F

def collect_addresses(img, scale=1, ox=0, oy=0):
    addr_map = {}
    for y in range(img.height):
        for x in range(img.width):
            sx = x*scale + ox
            sy = y*scale + oy
            if sx<0 or sy<0 or sx>=WINW or sy>=WINH:
                continue
            idx = nearest_palette_index(img.getpixel((x,y)))
            addr = sy*WINW + sx
            addr_map[addr] = idx
    return addr_map

def write_grouped_gif(infile, outfile, frame_delay, scale=1, ox=0, oy=0):
    img = Image.open(infile)
    prev_frame = {}
    lines = []
    lines.append(f"; Generated from {os.path.basename(infile)} (GIF)")
    lines.append(f"; Optimized: grouped by color + skip redundant clears")
    lines.append(f"; Frame delay: {frame_delay}ms")

    frame_count = 0
    for frame in ImageSequence.Iterator(img):
        frame_count += 1
        frame = frame.convert("RGB")
        if scale!=1:
            frame = frame.resize((frame.width*scale, frame.height*scale), Image.NEAREST)
        cur_frame = collect_addresses(frame, scale, ox, oy)

        # Collect clears: only pixels that were colored last frame and now are black
        clear_addrs = [addr for addr in prev_frame if prev_frame[addr]!=0 and cur_frame.get(addr,0)==0]
        if clear_addrs:
            lines.append(f"; Frame {frame_count} - clearing old pixels ({len(clear_addrs)})")
            lines.append("loadI 1 0 ; r1=black")
            for a in sorted(clear_addrs):
                lines.append(f"storeR 1 {a}")

        # Draw current pixels grouped by color
        lines.append(f"; Frame {frame_count} - drawing new pixels")
        color_to_addrs = defaultdict(list)
        for addr,color in cur_frame.items():
            # Only draw if color changed or was black
            if color==0: 
                continue
            if prev_frame.get(addr, 0) != color:
                color_to_addrs[color].append(addr)

        for color in sorted(color_to_addrs):
            lines.append(f"loadI 1 {color}")
            for a in sorted(color_to_addrs[color]):
                lines.append(f"storeR 1 {a}")

        lines.append("draw ; update screen")
        lines.append(f"delay {frame_delay} ; wait {frame_delay}ms")

        prev_frame = cur_frame.copy()

    lines.append("jump 0 ; loop forever")

    with open(outfile,"w") as f:
        f.write("\n".join(lines))
    print(f"Wrote {outfile}. Frames: {frame_count}, lines: {len(lines)}")

if __name__=="__main__":
    if len(sys.argv)<4:
        print("Usage: python gif2dj_optimized.py in.gif out.dj <frame_delay_ms> [scale] [offset_x] [offset_y]")
        sys.exit(1)
    infile = sys.argv[1]
    outfile = sys.argv[2]
    frame_delay = int(sys.argv[3])
    scale = int(sys.argv[4]) if len(sys.argv)>4 else 1
    ox = int(sys.argv[5]) if len(sys.argv)>5 else 0
    oy = int(sys.argv[6]) if len(sys.argv)>6 else 0
    write_grouped_gif(infile, outfile, frame_delay, scale, ox, oy)
