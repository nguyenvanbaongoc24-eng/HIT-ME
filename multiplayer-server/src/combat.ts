// Port of Unity CombatRules / RoundedRectangleArenaGeometry. Feet coordinates, y-up.
export interface Point { x:number; y:number }
export interface Action { id:string; position:Point; direction:Point; canThrow:boolean }
export interface Fighter { id:string; hp:number }
export const rules = Object.freeze({ width:2000,height:3500,corner:300,playerRadius:90,projectileRadius:30,maxHp:3,placementMs:5000,revealMs:300,throwMs:700,resultMs:600 });
export function valid(p:Point, clearance:number=rules.playerRadius) {
  if(!p || !Number.isFinite(p.x)||!Number.isFinite(p.y))return false;
  const a=rules.width/2-clearance,b=rules.height/2-clearance,r=Math.max(0,rules.corner-clearance);
  const x=Math.abs(p.x)-(a-r),y=Math.abs(p.y)-(b-r);
  return Math.hypot(Math.max(x,0),Math.max(y,0))+Math.min(Math.max(x,y),0)-r<=1e-8;
}
export function normalize(p:Point):Point {const n=Math.hypot(p?.x,p?.y);if(!Number.isFinite(n)||n<1e-9)throw Error('invalid_aim');return {x:p.x/n,y:p.y/n};}
export function wall(o:Point,d:Point,clearance=rules.projectileRadius):Point {
  if(!valid(o,clearance))throw Error('invalid_origin'); d=normalize(d);
  const a=rules.width/2-clearance,b=rules.height/2-clearance,r=Math.max(0,rules.corner-clearance);let best=-1;
  for(const s of [-1,1]) {
    if(Math.abs(d.x)>1e-12){const t=(s*a-o.x)/d.x;if(t>=-1e-8&&Math.abs(o.y+d.y*t)<=b-r+1e-8)best=Math.max(best,t);}
    if(Math.abs(d.y)>1e-12){const t=(s*b-o.y)/d.y;if(t>=-1e-8&&Math.abs(o.x+d.x*t)<=a-r+1e-8)best=Math.max(best,t);}
  }
  for(const sx of [-1,1])for(const sy of [-1,1]) {
    const ox=o.x-sx*(a-r),oy=o.y-sy*(b-r),q=ox*d.x+oy*d.y,disc=q*q-(ox*ox+oy*oy-r*r);
    if(disc < -1e-8)continue;
    for(const s of [-1,1]){const t=-q+s*Math.sqrt(Math.max(0,disc)),x=o.x+d.x*t,y=o.y+d.y*t;if(t>=-1e-8&&sx*x>=a-r-1e-8&&sy*y>=b-r-1e-8)best=Math.max(best,t);}
  }
  if(best< -1e-8)throw Error('no_exit');return {x:o.x+d.x*Math.max(0,best),y:o.y+d.y*Math.max(0,best)};
}
export function resolve(fighters:Fighter[],input:Action[]) {
  const people=[...fighters].sort((a,b)=>a.id<b.id?-1:a.id>b.id?1:0),actions=[...input].sort((a,b)=>a.id<b.id?-1:a.id>b.id?1:0);
  const alive=people.filter(p=>p.hp>0);
  if(new Set(people.map(p=>p.id)).size!==people.length||people.some(p=>!p.id||!Number.isInteger(p.hp)||p.hp<0||p.hp>rules.maxHp)||actions.length!==alive.length||new Set(actions.map(a=>a.id)).size!==actions.length||actions.some(a=>!alive.some(p=>p.id===a.id)||!valid(a.position)))throw Error('invalid_actions');
  const damage=new Map(people.map(p=>[p.id,0]));const throws=[];
  for(const a of actions){if(!a.canThrow)continue;const d=normalize(a.direction),boundary=wall(a.position,d),limit=(boundary.x-a.position.x)*d.x+(boundary.y-a.position.y)*d.y;let target:Action|undefined,nearest=Infinity;
    for(const b of actions){if(b.id===a.id)continue;const x=b.position.x-a.position.x,y=b.position.y-a.position.y,t=x*d.x+y*d.y;
      if(t>1e-9&&t<=limit+1e-9&&Math.abs(x*d.y-y*d.x)<=rules.playerRadius+rules.projectileRadius+1e-9&&t<nearest){target=b;nearest=t;}}
    if(target)damage.set(target.id,damage.get(target.id)!+1);
    throws.push({thrower:a.id,target:target?.id??null,damage:target?1:0,origin:a.position,end:target?{x:a.position.x+d.x*nearest,y:a.position.y+d.y*nearest}:boundary});
  }
  const health=people.map(p=>({id:p.id,before:p.hp,after:Math.max(0,p.hp-damage.get(p.id)!)})),survivors=health.filter(p=>p.after>0);
  return {throws,health,outcome:survivors.length===0?'Draw':survivors.length===1?'Winner':'Ongoing',winner:survivors.length===1?survivors[0].id:null};
}
