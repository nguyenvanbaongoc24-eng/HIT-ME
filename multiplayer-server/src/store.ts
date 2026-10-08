import { DatabaseSync } from 'node:sqlite';
import { randomUUID,randomBytes,createHash } from 'node:crypto';
export const modes=[{id:'classic',enabled:true},{id:'private',enabled:true},...['team','treasure','coop','chaos','tournament'].map(id=>({id,enabled:false}))];
export const catalog=[{id:'dep-to-ong',rarity:'Common',slot:'weapon'},{id:'chao',rarity:'Common',slot:'weapon'},{id:'vot',rarity:'Common',slot:'weapon'},...['LangQueBacBo','ChoVietNam','VinhHaLong','CoDoHue','ChoNoiCaiRang','TayBac'].map(id=>({id,rarity:'Common',slot:'map'}))];
// Trial economy only. Private room cap is an adjustable development policy.
export const rewardPolicy={win:{coins:100,xp:50},lose:{coins:40,xp:20},draw:{coins:65,xp:30},privateDailyCap:3};
const hash=(s:string)=>createHash('sha256').update(s).digest('hex');
export class Store {
  db:DatabaseSync;
  constructor(path:string){this.db=new DatabaseSync(path);this.db.exec(`PRAGMA journal_mode=WAL; PRAGMA foreign_keys=ON;
    CREATE TABLE IF NOT EXISTS profiles(id TEXT PRIMARY KEY,name TEXT NOT NULL,avatar TEXT NOT NULL DEFAULT 'Char01_Player',xp INTEGER NOT NULL DEFAULT 0,coins INTEGER NOT NULL DEFAULT 0,equipped TEXT NOT NULL DEFAULT 'dep-to-ong');
    CREATE TABLE IF NOT EXISTS sessions(token_hash TEXT PRIMARY KEY,player TEXT NOT NULL REFERENCES profiles(id),expires INTEGER NOT NULL);
    CREATE TABLE IF NOT EXISTS inventory(player TEXT NOT NULL REFERENCES profiles(id),item TEXT NOT NULL,quantity INTEGER NOT NULL DEFAULT 1,PRIMARY KEY(player,item));
    CREATE TABLE IF NOT EXISTS rewards(match TEXT NOT NULL,player TEXT NOT NULL REFERENCES profiles(id),outcome TEXT NOT NULL,coins INTEGER NOT NULL,xp INTEGER NOT NULL,private INTEGER NOT NULL,created INTEGER NOT NULL,PRIMARY KEY(match,player));
    CREATE TABLE IF NOT EXISTS events(id TEXT PRIMARY KEY,player TEXT NOT NULL,type TEXT NOT NULL,created INTEGER NOT NULL);
    CREATE TABLE IF NOT EXISTS quests(player TEXT NOT NULL,period TEXT NOT NULL,kind TEXT NOT NULL,progress INTEGER NOT NULL DEFAULT 0,claimed INTEGER NOT NULL DEFAULT 0,PRIMARY KEY(player,period,kind));`);}
  guest(name:string,token?:string){const now=Date.now();if(token){const s=this.db.prepare('SELECT player FROM sessions WHERE token_hash=? AND expires>?').get(hash(token),now) as {player:string}|undefined;if(!s)throw Error('invalid_session');return {id:s.player,token};}
    const id=randomUUID();token=randomBytes(32).toString('base64url');name=(name||'Quest').trim().slice(0,24);if(!name)throw Error('invalid_name');
    this.db.exec('BEGIN IMMEDIATE');try{this.db.prepare('INSERT INTO profiles(id,name) VALUES(?,?)').run(id,name);this.db.prepare('INSERT INTO sessions VALUES(?,?,?)').run(hash(token),id,now+30*86400000);for(const item of ['dep-to-ong','chao','vot'])this.db.prepare('INSERT INTO inventory(player,item) VALUES(?,?)').run(id,item);this.db.exec('COMMIT');}catch(e){this.db.exec('ROLLBACK');throw e;}return {id,token};}
  profile(id:string):Record<string,any>{const p=this.db.prepare('SELECT * FROM profiles WHERE id=?').get(id) as Record<string,any>;if(!p)throw Error('unknown_player');return {...p,level:1+Math.floor(p.xp/100),inventory:this.db.prepare('SELECT item,quantity FROM inventory WHERE player=?').all(id),materials:[],craftingRecipes:[],history:this.db.prepare('SELECT * FROM rewards WHERE player=? ORDER BY created DESC LIMIT 20').all(id),quests:this.quests(id),expiredQuests:this.questArchive(id)};}
  questArchive(id:string){const periods=new Set(this.quests(id).map(q=>q.period));return this.db.prepare('SELECT period,kind,progress,claimed FROM quests WHERE player=?').all(id).filter(row=>!periods.has(String(row.period))).map(row=>({...row,state:row.claimed?'Claimed':'Expired'}));}
  quests(id:string){const today=new Date().toISOString().slice(0,10),week=Math.floor(Date.now()/(7*86400000)).toString();return ['MatchCompleted','MatchWon','ValidHit','MapPlayed','FriendMatchCompleted'].flatMap(kind=>[{period:today,kind,target:kind==='MatchCompleted'?3:1},{period:week,kind,target:kind==='MatchCompleted'?10:3}]).map(q=>{const row=this.db.prepare('SELECT progress,claimed FROM quests WHERE player=? AND period=? AND kind=?').get(id,q.period,q.kind) as any;const progress=row?.progress??0;return {...q,progress,state:row?.claimed?'Claimed':progress>=q.target?'Completed':'Active'};});}
  event(id:string,player:string,type:string){const own=!this.db.isTransaction;if(own)this.db.exec("BEGIN IMMEDIATE");try{this.applyEvent(id,player,type);if(own)this.db.exec("COMMIT");}catch(e){if(own)this.db.exec("ROLLBACK");throw e;}}
  private applyEvent(id:string,player:string,type:string){const changed=this.db.prepare('INSERT OR IGNORE INTO events VALUES(?,?,?,?)').run(id,player,type,Date.now()).changes;if(!changed)return;
    for(const q of this.quests(player).filter(q=>q.kind===type))this.db.prepare('INSERT INTO quests(player,period,kind,progress) VALUES(?,?,?,1) ON CONFLICT(player,period,kind) DO UPDATE SET progress=progress+1').run(player,q.period,type);}
  reward(match:string,players:string[],winner:string|null,privateRoom:boolean,validMatch:boolean,map='LangQueBacBo'){
    if(!validMatch||players.length<2||players.length>6||new Set(players).size!==players.length||winner!==null&&!players.includes(winner))throw Error('invalid_match');
    this.db.exec('BEGIN IMMEDIATE');try{for(const player of players){const outcome=winner===null?'draw':winner===player?'win':'lose';let award=rewardPolicy[outcome];
      const count=this.db.prepare('SELECT count(*) AS n FROM rewards WHERE player=? AND private=1 AND coins>0 AND created>=?').get(player,Math.floor(Date.now()/86400000)*86400000) as any;
      if(privateRoom&&count.n>=rewardPolicy.privateDailyCap)award={coins:0,xp:0};
      if(!this.db.prepare('INSERT OR IGNORE INTO rewards VALUES(?,?,?,?,?,?,?)').run(match,player,outcome,award.coins,award.xp,privateRoom?1:0,Date.now()).changes)continue;
      this.db.prepare('UPDATE profiles SET coins=coins+?,xp=xp+? WHERE id=?').run(award.coins,award.xp,player);
      this.db.prepare('INSERT OR IGNORE INTO inventory(player,item) VALUES(?,?)').run(player,map);
      for(const type of ['MatchCompleted','MapPlayed',...(privateRoom?['FriendMatchCompleted']:[]),...(winner===player?['MatchWon']:[])])this.event(`${match}:${player}:${type}`,player,type);
    }this.db.exec('COMMIT');}catch(e){this.db.exec('ROLLBACK');throw e;}
  }
  equip(player:string,item:string){if(!catalog.some(i=>i.id===item&&i.slot==='weapon')||!this.db.prepare('SELECT 1 FROM inventory WHERE player=? AND item=?').get(player,item))throw Error('not_owned');this.db.prepare('UPDATE profiles SET equipped=? WHERE id=?').run(item,player);}
  claim(player:string,period:string,kind:string){const q=this.quests(player).find(q=>q.period===period&&q.kind===kind);if(!q||q.state!=='Completed')throw Error('quest_not_claimable');this.db.prepare('UPDATE quests SET claimed=1 WHERE player=? AND period=? AND kind=? AND claimed=0').run(player,period,kind);/* No invented quest payout: claim records completion only. */}
  close(){this.db.close();}
}
