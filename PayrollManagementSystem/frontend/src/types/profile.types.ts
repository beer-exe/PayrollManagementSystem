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
  
  soTaiKhoan?: string | null;
  tenNganHang?: string | null;
  maSoThue?: string | null;
  
  luongP1?: number | null;
  heSoP2?: number | null;
  soHopDong?: string | null;
  loaiHopDong?: string | null;
  ngayBatDauHopDong?: string | null;
  
  thanNhans: ThanNhanDto[];
  idPb?: string | null;
}

export interface ThanNhanDto {
  maDinhDanh?: string;
  tenTn: string;
  ngaySinh: string | null;
  idMqh?: string | null;
  moiQuanHe: string | null;
}