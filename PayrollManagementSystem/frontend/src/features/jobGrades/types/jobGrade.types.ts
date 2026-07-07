export interface JobGrade {
  idNgachLuong: string;
  tenNgachLuong: string;
  moTa: string | null;
  trangThai: number;
}

export interface CreateJobGradeDto {
  tenNgachLuong: string;
  moTa: string | null;
}

export interface UpdateJobGradeDto {
  idNgachLuong: string;
  tenNgachLuong: string;
  moTa: string | null;
  trangThai: number;
}
