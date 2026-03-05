export interface IPagination<T> {
  pageSize: number;
  pageIndex: number;
  count: number;
  data: T[];
}
