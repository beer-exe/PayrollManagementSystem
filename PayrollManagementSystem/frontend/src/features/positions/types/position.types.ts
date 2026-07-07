export interface PositionDto {
  idChucVu: string;
  tenChucVu: string;
  moTaCongViec?: string;
  idNgachLuong?: string;
  tenNgachLuong?: string;
  trangThai: 'HOAT_DONG' | 'NGUNG_HOAT_DONG';
  idPhongBan: string;
  tenPhongBan?: string;
  idChucVuQuanLy?: string;
  tenChucVuQuanLy?: string;
}

export interface CreatePositionCommand {
  idChucVu: string;
  tenChucVu: string;
  moTaCongViec?: string;
  idNgachLuong?: string;
  idPhongBan: string;
  idChucVuQuanLy?: string;
}

export interface UpdatePositionCommand {
  idChucVu: string;
  tenChucVu: string;
  moTaCongViec?: string;
  idNgachLuong?: string;
  idPhongBan: string;
  idChucVuQuanLy?: string;
}