export enum TrangThaiNhanVien {
  DANG_LAM_VIEC = 'DANG_LAM_VIEC',
  DA_NGHI_VIEC = 'DA_NGHI_VIEC',
  THAI_SAN = 'THAI_SAN',
  TAM_DINH_CHI = 'TAM_DINH_CHI'
}

export interface ChangeStatusDto {
  cccd: string;
  trangThaiMoi: TrangThaiNhanVien;
  lyDo: string;
}

export interface CreateEmployeeCommand {
  cccd: string;
  hoTen: string;
  email?: string;
  sdt?: string;
  idPb: string;
  soHopDong: string;
  loaiHopDong: string;
  luongCoBan: number;
  ngayBatDauHopDong: string;
  soQuyetDinh: string;
  idChucVu: string;
}

export interface UpdateEmployeeCommand {
  cccd: string;
  hoTen: string;
  gioiTinh?: boolean | null;
  sdt?: string | null;
  email?: string | null;
  ngaySinh?: string | null;
  danToc?: string | null;
  diaChi?: string | null;
  chuyenNganh?: string | null;
  soBhxh?: string | null;
  soBhyt?: string | null;
}