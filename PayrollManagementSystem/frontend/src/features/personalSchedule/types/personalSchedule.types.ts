export interface MyScheduleDayDto {
  ngay: string; // "YYYY-MM-DD"
  thu: string;
  loaiNgay: string;
  tenNgayNghi: string | null;
  idCaLamViec: string | null;
  tenCa: string | null;
  gioBatDau: string | null; // "HH:mm:ss"
  gioKetThuc: string | null;
  xuyenNgay: boolean;
  laCaDuocPhanCong: boolean;
  coNghiPhep: boolean;
  loaiNghiPhep: string | null;
}
