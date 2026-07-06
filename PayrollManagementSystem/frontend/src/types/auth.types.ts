export interface ApiResponse<T> {
  succeeded: boolean;
  message: string | null;
  errors: string[] | null;
  data: T;
}

export interface AuthResponseDto {
  userId: string;
  fullName: string;
  email: string;
  accessToken: string;
  refreshToken: string;
  hasDirectReports: boolean;
}

export interface LoginRequestDto {
  tenTaiKhoan: string;
  matKhau: string;
}

export interface UserProfile {
  id: string;
  name: string;
  email: string;
  role: string;
  hasDirectReports: boolean;
}