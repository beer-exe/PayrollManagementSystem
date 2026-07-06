
export interface UserDto {
  idTaiKhoan: string;
  tenTaiKhoan: string;
  email: string;
  hoTen: string;
  tenVaiTro: string;
  idVaiTro: string;
  trangThai: 'HOAT_DONG' | 'KHOA' | 'CHO_XAC_NHAN';
}

export interface RoleDto {
  idVaiTro: string;
  tenVaiTro: string;
}

export interface CreateUserCommand {
  tenTaiKhoan: string;
  matKhau: string;
  idVaiTro: string;
  cccd: string;
}

export interface UpdateUserRoleCommand {
  idTaiKhoan: string;
  idVaiTroMoi: string;
}

export interface ResetPasswordCommand {
  idTaiKhoan: string;
  newPassword: string;
}

export interface EmployeeNoAccount {
  cccd: string;
  hoTen: string;
  tenPhongBan?: string | null;
}