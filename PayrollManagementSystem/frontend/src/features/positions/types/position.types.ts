export interface PositionDto {
  idChucVu: string;
  tenChucVu: string;
  moTaCongViec?: string;
  trangThai: 'HOAT_DONG' | 'NGUNG_HOAT_DONG';
}

export interface CreatePositionCommand {
  idChucVu: string;
  tenChucVu: string;
  moTaCongViec?: string;
}

export interface UpdatePositionCommand {
  idChucVu: string;
  tenChucVu: string;
  moTaCongViec?: string;
}