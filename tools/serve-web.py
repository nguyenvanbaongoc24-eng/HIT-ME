"""Local-only Unity Web host; use python tools/serve-web.py --port 8790."""
import argparse
from http.server import ThreadingHTTPServer, SimpleHTTPRequestHandler
from pathlib import Path
parser=argparse.ArgumentParser();parser.add_argument('--port',type=int,default=8790);args=parser.parse_args()
root=Path(__file__).resolve().parents[1]/'unity-client/Builds/Web'
if not (root/'index.html').is_file():raise SystemExit('Web build missing; run tools/build-web.ps1 first.')
class Handler(SimpleHTTPRequestHandler):
 def __init__(self,*a,**kw):super().__init__(*a,directory=str(root),**kw)
 def end_headers(self):self.send_header('Cache-Control','no-cache');super().end_headers()
 def guess_type(self,path):
  if path.endswith('.unityweb'):return 'application/octet-stream'
  if path.endswith('.wasm'):return 'application/wasm'
  return super().guess_type(path)
print(f'HIT ME: http://127.0.0.1:{args.port}/',flush=True)
ThreadingHTTPServer(('127.0.0.1',args.port),Handler).serve_forever()
