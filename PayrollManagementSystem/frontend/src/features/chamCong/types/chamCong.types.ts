// LoaiNgayCong description values từ backend enum (GetDescription())
export type LoaiNgayCong =
  | 'Làm đủ ca'
  | 'Nửa ca'
  | 'Đi trễ / Về sớm'
  | 'Vắng có phép'
  | 'Vắng không phép'
  | 'Nghỉ lễ'
  | 'Nghỉ cuối tuần';

export type TrangThaiChamCong =
  | 'Chưa xác nhận'
  | 'Đã xác nhận'
  | 'Cần giải trình';

export interface ChamCongDto {
  id: string;
  cccdNhanVien: string;
  hoTenNhanVien: string;
  ngayChamCong: string;    // "YYYY-MM-DD"
  gioVao?: string | null;  // "HH:mm"
  gioRa?: string | null;   // "HH:mm"
  soGioLamThucTe: number;
  soNgayCong: number;
  loaiNgayCong: LoaiNgayCong;
  trangThai: TrangThaiChamCong;
  isNhapTay: boolean;
  ghiChu?: string | null;
  ngayTao?: string | null;
}

export interface ChamCongSummaryDto {
  cccdNhanVien: string;
  hoTenNhanVien: string;
  tenPhongBan?: string | null;
  thang: number;
  nam: number;
  ngayCongChuan: number;
  tongNgayCongThucTe: number;
  ngayNghiLe: number;
  ngayNghiCuoiTuan: number;
  ngayVangKhongPhep: number;
  ngayCanGiaiTrinh: number;
}

export interface ImportChamCongResultDto {
  tongSoDong: number;
  thanhCong: number;
  thatBai: number;
  loiNhap: string[];
}

export interface CreateChamCongRequest {
  cccdNhanVien: string;
  ngayChamCong: string;   // "YYYY-MM-DD"
  gioVao?: string | null; // "HH:mm"
  gioRa?: string | null;  // "HH:mm"
  ghiChu?: string;
}

export interface UpdateChamCongRequest {
  gioVao?: string | null;
  gioRa?: string | null;
  ghiChu?: string;
}

export interface ChamCongFilterParams {
  thang: number;
  nam: number;
  cccd?: string;
}
