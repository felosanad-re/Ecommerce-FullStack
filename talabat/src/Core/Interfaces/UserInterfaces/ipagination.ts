export interface IPagination<T> {
  pageSize: number;
  pageIndex: number;
  count: number;
  products: T[];
}
