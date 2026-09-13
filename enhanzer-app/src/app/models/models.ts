export interface LoginRequest {
  email: string;
  password: string;
}

export interface Location {
  locationCode: string;
  locationName: string;
}

export interface LoginResponse {
  token: string;
  locations: Location[];
}

export interface PurchaseBillRequest {
  itemName: string;
  batchName: string;
  standardCost: number;
  standardPrice: number;
  quantity: number;
  discountPercentage: number;
}

export interface PurchaseBillItem extends PurchaseBillRequest {
  totalCost: number;
  totalSelling: number;
}