import { randomBytes,randomUUID } from 'node:crypto';
import { Action,Point,Fighter,valid,normalize,resolve,rules } from './combat.js';
import { Store } from './store.js';
export interface Member extends Fighter {name:string;weapon:string;avatar:string;ready:boolean;connected:boolean;disconnectedAt:number;seq:number;action:Action|null;locked:boolean;previous:Point}
export interface MatchStats {id:string;throws:number;hits:number;misses:number;received:number}
export interface Room {id:string;code:string;owner:string;private:boolean;expires:number;players:Member[];phase:string;round:number;deadline:number;match:string;resolution:ReturnType<typeof resolve>|null;started:number;validActions:number;submitted:string[];stats:MatchStats[]}
export class Rooms {
  rooms=new Map<string,Room>();membership=new Map<string,string>();
  private seed=2026;
  private randomPosition(){const next=()=>{this.seed^=this.seed<<13;this.seed^=this.seed>>>17;this.seed^=this.seed<<5;return (this.seed>>>0)/4294967296;};for(let i=0;i<64;i++){const p={x:(next()*2-1)*(rules.width/2-rules.playerRadius),y:(next()*2-1)*(rules.height/2-rules.playerRadius)};if(valid(p))return p;}return {x:0,y:0};}
  constructor(public store:Store,public now=()=>Date.now(),public proposedTimeout=true,public graceMs=30000){}
  room(player:string){const room=this.rooms.get(this.membership.get(player)??'');if(!room)throw Error('not_in_room');return room;}
  create(id:string,privateRoom=true){if(this.membership.has(id))throw Error('already_in_room');const r:Room={id:randomUUID(),code:randomBytes(4).toString('hex').toUpperCase(),owner:id,private:privateRoom,expires:this.now()+30*60000,players:[],phase:'Waiting',round:0,deadline:0,match:'',resolution:null,started:0,validActions:0,submitted:[],stats:[]};this.rooms.set(r.id,r);this.add(r,id);return r;}
  add(r:Room,id:string){if(this.membership.has(id)||r.phase!=='Waiting'||r.players.length>=6||r.expires<this.now())throw Error('room_unavailable');const p=this.store.profile(id);r.players.push({id,name:p.name,weapon:p.equipped,avatar:p.avatar,hp:rules.maxHp,ready:false,connected:true,disconnectedAt:0,seq:0,action:null,locked:false,previous:this.randomPosition()});this.membership.set(id,r.id);return r;}
  join(id:string,code:string){const r=[...this.rooms.values()].find(r=>r.code===code.toUpperCase());if(!r)throw Error('invalid_code');return this.add(r,id);}
  quick(id:string){const r=[...this.rooms.values()].find(r=>!r.private&&r.phase==='Waiting'&&r.players.length<6&&r.expires>=this.now());return r?this.add(r,id):this.create(id,false);}
  ready(id:string,ready:boolean){const r=this.room(id);if(!['Waiting','Countdown'].includes(r.phase))throw Error('match_started');r.players.find(p=>p.id===id)!.ready=ready;
    if(r.players.length>=2&&r.players.every(p=>p.ready&&p.connected)){if(r.phase!=='Countdown'){r.phase='Countdown';r.deadline=this.now()+3000;}}else{r.phase='Waiting';r.deadline=0;}}
  leave(id:string){const r=this.room(id);if(!['Waiting','Countdown','MatchResult'].includes(r.phase))throw Error('match_in_progress');r.players=r.players.filter(p=>p.id!==id);this.membership.delete(id);if(!r.players.length)this.rooms.delete(r.id);else {r.owner=r.players[0].id;r.phase='Waiting';for(const p of r.players)p.ready=false;}}
  disconnect(id:string){if(!this.membership.has(id))return;const r=this.room(id),p=r.players.find(p=>p.id===id)!;p.connected=false;p.disconnectedAt=this.now();if(r.phase==='Countdown'){r.phase='Waiting';r.deadline=0;}}
  reconnect(id:string){if(!this.membership.has(id))return;const p=this.room(id).players.find(p=>p.id===id)!;p.connected=true;p.disconnectedAt=0;}
  action(id:string,seq:number,type:string,value:Point|null,round:number){const r=this.room(id),p=r.players.find(p=>p.id===id)!;if(!Number.isSafeInteger(seq)||seq<1)throw Error('invalid_sequence');if(seq<=p.seq)return {duplicate:true};
    if(r.phase!=='Placement'||r.round!==round||this.now()>=r.deadline||p.hp===0||p.locked)throw Error('action_locked');
    if(type==='place'){if(!value||!valid(value))throw Error('invalid_position');p.action={id,position:{...value},direction:{x:0,y:0},canThrow:false};}
    else if(type==='aim'){if(!p.action||!value)throw Error('place_first');p.action.direction=normalize(value);p.action.canThrow=true;}
    else if(type==='lock'){if(!p.action?.canThrow)throw Error('incomplete_action');p.locked=true;r.validActions++;if(!r.submitted.includes(id))r.submitted.push(id);}
    else throw Error('unknown_action');p.seq=seq;
    if(r.players.filter(p=>p.hp>0).every(p=>p.locked))this.reveal(r);return {duplicate:false};}
  next(r:Room){r.phase='Placement';r.round++;r.deadline=this.now()+rules.placementMs;for(const p of r.players){p.action=null;p.locked=false;}}
  reveal(r:Room){for(const p of r.players.filter(p=>p.hp>0)){if(p.action?.canThrow&&!r.submitted.includes(p.id))r.submitted.push(p.id);if(!p.action?.canThrow){if(!this.proposedTimeout){r.phase='RequirementsBlocked';return;}p.action={id:p.id,position:p.previous,direction:{x:0,y:0},canThrow:false};}p.locked=true;p.previous=p.action!.position;}
    r.resolution=resolve(r.players,r.players.filter(p=>p.hp>0).map(p=>p.action!));r.phase='Reveal';r.deadline=this.now()+rules.revealMs;}
  tick(){for(const r of [...this.rooms.values()]){
    // No HP loss / forfeiture inferred for unresolved disconnect-expiry policy.
    for(const p of [...r.players])if(!p.connected&&this.now()-p.disconnectedAt>=this.graceMs&&['Waiting','Countdown','MatchResult'].includes(r.phase)){this.leave(p.id);}
    if(!this.rooms.has(r.id))continue;
    if(r.phase==='Waiting'&&r.expires<this.now()){for(const p of r.players)this.membership.delete(p.id);this.rooms.delete(r.id);continue;}
    if(this.now()<r.deadline)continue;
    if(r.phase==='Countdown'){r.match=randomUUID();r.started=this.now();r.validActions=0;r.submitted=[];r.stats=r.players.map(p=>({id:p.id,throws:0,hits:0,misses:0,received:0}));r.round=0;for(const p of r.players){p.hp=rules.maxHp;p.previous=this.randomPosition();}this.next(r);}
    else if(r.phase==='Placement')this.reveal(r);
    else if(r.phase==='Reveal'){r.phase='Throw';r.deadline=this.now()+rules.throwMs;}
    else if(r.phase==='Throw'){for(const h of r.resolution!.health)r.players.find(p=>p.id===h.id)!.hp=h.after;r.phase='RoundResult';r.deadline=this.now()+rules.resultMs;
      for(const result of r.resolution!.throws){const row=r.stats.find(s=>s.id===result.thrower)!;row.throws++;if(result.target){row.hits++;r.stats.find(s=>s.id===result.target)!.received++;}else row.misses++;}
      for(const hit of r.resolution!.throws.filter(t=>t.target))this.store.event(`${r.match}:${r.round}:${hit.thrower}:ValidHit`,hit.thrower,'ValidHit');}
    else if(r.phase==='RoundResult'){if(r.resolution!.outcome==='Ongoing')this.next(r);else {if(r.players.every(p=>r.submitted.includes(p.id)))this.store.reward(r.match,r.players.map(p=>p.id),r.resolution!.winner,r.private,true);r.phase='MatchResult';r.deadline=0;}}
  }}
  snapshot(id:string){if(!this.membership.has(id))return null;const r=this.room(id);const hidden=['Placement','Waiting','Countdown','RequirementsBlocked'].includes(r.phase);
    return {id:r.id,code:r.code,owner:r.owner,private:r.private,phase:r.phase,round:r.round,deadline:r.deadline,serverTime:this.now(),match:r.match,timeoutPolicy:this.proposedTimeout?'PROPOSED_STAY_SKIP':'UNCONFIRMED',players:r.players.map(p=>({id:p.id,name:p.name,weapon:p.weapon,avatar:p.avatar,hp:p.hp,ready:p.ready,connected:p.connected,locked:p.locked,seq:p.id===id?p.seq:undefined,action:hidden&&p.id!==id?null:p.action})),resolution:hidden?null:r.resolution,stats:r.phase==='MatchResult'?r.stats:[]};}
}
