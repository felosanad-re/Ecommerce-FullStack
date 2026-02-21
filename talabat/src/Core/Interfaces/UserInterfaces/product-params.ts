export class ProductParams {
  search?: string;
  pageIndex = 1;
  pageSize = 12;
  brandId?: number;
  categoryId?: number;
  isInStock?: boolean; // To Check If Product In Stock
  isDeleted?: boolean; // To Check If Product Is Deleted
}
