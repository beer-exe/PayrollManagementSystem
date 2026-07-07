export interface DepartmentDto {
  idPb: string;
  tenPb: string;
}
export interface EmployeeInDepartmentDto {
  cccd: string;
  hoTen: string;
  email: string;
  tenChucVu: string;
  trangThai: string;
  tenTrangThai?: string;
  ngayVaoLam: string;
}

export interface CreateDepartmentCommand {
  idPb: string;
  tenPb: string;
}
export interface TransferEmployeeCommand {
  cccd: string;
  idPbMoi: string;
  idChucVuMoi: string;
  idBacLuongMoi: string;
  soQuyetDinh: string;
  ngayHieuLuc: string;
  nguoiKy?: string;
}

export interface AdjustSalaryCommand {
  soQuyetDinh: string;
  cccd: string;
  idBacLuongMoi: string;
  ngayHieuLuc: string;
  lyDo?: string;
}

export interface ChangePositionCommand {
  soQuyetDinh: string;
  cccd: string;
  idChucVuMoi: string;
  idBacLuongMoi: string;
  ngayHieuLuc: string;
  lyDo?: string;
}