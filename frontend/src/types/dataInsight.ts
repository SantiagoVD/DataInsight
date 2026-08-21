export interface CreateUploadResponse {
  UploadUrl: string;
  ObjectKey: string;
}

export interface CreateJobResponse {
  JobId: string;
  Status: string;
}

export interface ProductMetric {
  Product: string;
  Quantity: number;
  Revenue: number;
}

export interface ProcessingResult {
  JobId: string;
  FileName: string;
  Status: string;
  TotalRecords: number;
  TotalUnits: number;
  TotalRevenue: number;
  TopSellingProduct: string;
  HighestRevenueProduct: string;
  Products: ProductMetric[];
  ProcessedAt: string;
}

export type ProcessingJobResponse = CreateJobResponse | ProcessingResult;
