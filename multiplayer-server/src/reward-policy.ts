// Preserve the existing development economy. This is not a production payout approval.
export const rewardPolicy={version:'trial-v1',win:{coins:100,xp:50},lose:{coins:40,xp:20},draw:{coins:65,xp:30},privateDailyCap:3};
// User-supplied beta proposal is recorded but cannot be activated with unresolved rules.
export const betaRewardProposal={version:'beta-v1',enabled:false,completed:20,rankBonus:{1:30,2:15,3:10},confirmedElimination:5,dailyCurrencyCap:null,tieRule:null,drawRule:null,eliminationCreditRule:null};
