export interface UserProfileDetail {
  cccd: string;
  hoTen: string;
  gioiTinh: boolean | null;
  sdt: string | null;
  email: string | null;
  ngaySinh: string | null; // Format YYYY-MM-DD
  danToc: string | null;
  diaChi: string | null;
  chuyenNganh: string | null;
  ngayVaoLam: string | null;
  trangThai: string | null;
  soBhxh: string | null;
  soBhyt: string | null;
  tenPhongBan: string | null;
  tenChucVu: string | null;
  thanNhans: ThanNhanDto[];
}

export interface ThanNhanDto {
  tenTn: string;
  ngaySinh: string | null;
  moiQuanHe: string | null;
}