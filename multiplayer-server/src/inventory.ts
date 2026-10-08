// Data contracts only: recipes/material grants are pending confirmed economy requirements.
export type ItemRarity='Common'|'Uncommon'|'Rare'|'Epic'|'Legendary';
export interface ItemDefinition {id:string;nameVi:string;nameEn:string;rarity:ItemRarity;slot:'weapon'|'avatar'|'outfit'|'map'|'material';visualAsset:string}
export interface PlayerInventory {playerId:string;items:{itemId:string;quantity:number}[]}
export interface CosmeticEquipment {weaponId:string;avatarId:string;outfitId?:string}
export interface MapCollection {playerId:string;unlockedMapIds:string[]}
export interface MaterialWallet {playerId:string;balances:Record<string,number>}
export interface CraftingRecipe {id:string;inputs:{itemId:string;quantity:number}[];outputItemId:string;enabled:boolean}
export interface GameModeDefinition {id:string;minPlayers:number;maxPlayers:number;enabled:boolean;teamSize?:number}
