import { WebSocket } from 'ws';

const wssUrl = 'wss://hit-me-zj17.onrender.com/play';

console.log(`Connecting to remote WebSocket: ${wssUrl} ...`);
const ws = new WebSocket(wssUrl, {
  headers: {
    Origin: 'https://hit-me-game.vercel.app'
  }
});

ws.on('open', () => {
  console.log('[PASS] WebSocket connected successfully!');
  const helloReq = {
    type: 'hello',
    request: 'req_test_1',
    name: 'OnlineTester',
    token: null
  };
  console.log('Sending hello payload...');
  ws.send(JSON.stringify(helloReq));
});

ws.on('message', (data) => {
  console.log('[RECV]', data.toString());
  ws.close();
  process.exit(0);
});

ws.on('error', (err) => {
  console.error('[FAIL] WebSocket error:', err);
  process.exit(1);
});

setTimeout(() => {
  console.error('[TIMEOUT] No response in 10s');
  process.exit(1);
}, 10000);
