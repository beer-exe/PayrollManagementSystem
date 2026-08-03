export interface KhoanKhauTruDto {
  idKhoanKhauTru: string;
  tenKhoanKhauTru: string;
  /** Description string của enum: "Tỷ lệ phần trăm" | "Số tiền cố định" */
  loaiCongThuc: string;
  giaTri: number;
  ghiChu?: string;
  isActive: boolean;
  createdAt: string;
}

export type LoaiCongThuc = 'TY_LE_PHAN_TRAM' | 'SO_TIEN_CO_DINH';

export interface CreateKhoanKhauTruRequest {
  tenKhoanKhauTru: string;
  loaiCongThuc: LoaiCongThuc;
  giaTri: number;
  ghiChu?: string;
  isActive: boolean;
}

export interface UpdateKhoanKhauTruRequest {
  tenKhoanKhauTru: string;
  loaiCongThuc: LoaiCongThuc;
  giaTri: number;
  ghiChu?: string;
  isActive: boolean;
}

export interface KhoanKhauTruListResponse {
  succeeded: boolean;
  data: KhoanKhauTruDto[];
  message?: string;
}
