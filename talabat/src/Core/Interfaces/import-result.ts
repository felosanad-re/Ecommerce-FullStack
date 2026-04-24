export interface ImportResult<T> {
  totalRows: number;
  addedCount: number;
  skippedDuplicates: number;
  errors: string[];
  message: string;
  data: T[];
}
