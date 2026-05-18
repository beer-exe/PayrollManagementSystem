export interface AuthResponseDto {
  userId: string;
  fullName: string;
  email: string;
  accessToken: string;
  refreshToken: string;
}

export interface LoginRequestDto {
  email: string;
  passwordHash: string;
}

export interface UserProfile {
  id: string;
  name: string;
  email: string;
  role?: string;
}