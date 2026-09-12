export interface Product {
  id: string;
  sku: string;
  name: string;
  description: string | null;
  price: number;
  stock: number;
  createdAtUtc: string;
  updatedAtUtc: string | null;
}

export interface StockSummary {
  productId: string;
  sku: string;
  currentStock: number;
  updatedAtUtc: string | null;
}
