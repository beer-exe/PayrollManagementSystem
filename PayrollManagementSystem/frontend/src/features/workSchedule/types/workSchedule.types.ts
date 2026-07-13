export interface LichLamViecDto {
  idLich: string;
  nam: number;
  trangThai: string;
  tongNgay: number;
  tongNgayLam: number;
  tongNgayNghiCuoiTuan: number;
  tongNgayLe: number;
  ghiChu?: string | null;
  nguoiTao?: string | null;
  ngayTao?: string | null;
}

export interface ChiTietLichLamViecDto {
  id: string;
  ngay: string; // "YYYY-MM-DD"
  thu: string;
  loaiNgay: string; // Description from backend
  tenNgayNghi?: string | null;
  soGioLam: number;
}

export interface CreateLichLamViecRequest {
  nam: number;
  ghiChu?: string;
}

export interface ChiTietQueryParams {
  thang: number;
  pageNumber: number;
  pageSize: number;
}
