import { useState, useCallback } from 'react';
import { jobGradeApi } from '../api/jobGradeApi';
import { JobGrade, CreateJobGradeDto, UpdateJobGradeDto } from '../types/jobGrade.types';

export const useJobGrades = () => {
  const [jobGrades, setJobGrades] = useState<JobGrade[]>([]);
  const [loading, setLoading] = useState(false);
  const [toast, setToast] = useState<{ message: string; type: "success" | "error" } | null>(null);

  const showToast = (message: string, type: "success" | "error") => {
    setToast({ message, type });
  };

  const fetchJobGrades = useCallback(async () => {
    setLoading(true);
    try {
      const res = await jobGradeApi.getJobGrades();
      if (res.succeeded) setJobGrades(res.data);
    } catch (error) { const err = error as import('axios').AxiosError<{Message?: string}>;
      showToast(err?.response?.data?.Message || 'Lỗi tải danh sách ngạch lương', 'error');
    } finally {
      setLoading(false);
    }
  }, []);

  const createJobGrade = async (data: CreateJobGradeDto) => {
    try {
      const res = await jobGradeApi.createJobGrade(data);
      if (res.succeeded) {
        showToast('Thêm mới thành công!', 'success');
        return true;
      }
    } catch (error) { const err = error as import('axios').AxiosError<{Message?: string}>;
      showToast(err?.response?.data?.Message || 'Lỗi thêm mới', 'error');
    }
    return false;
  };

  const updateJobGrade = async (data: UpdateJobGradeDto) => {
    try {
      const res = await jobGradeApi.updateJobGrade(data);
      if (res.succeeded) {
        showToast('Cập nhật thành công!', 'success');
        return true;
      }
    } catch (error) { const err = error as import('axios').AxiosError<{Message?: string}>;
      showToast(err?.response?.data?.Message || 'Lỗi cập nhật', 'error');
    }
    return false;
  };

  const deleteJobGrade = async (id: string) => {
    try {
      const res = await jobGradeApi.deleteJobGrade(id);
      if (res.succeeded) {
        showToast('Xóa thành công!', 'success');
        return true;
      }
    } catch (error) { const err = error as import('axios').AxiosError<{Message?: string}>;
      showToast(err?.response?.data?.Message || 'Lỗi xóa', 'error');
    }
    return false;
  };

  return { jobGrades, loading, fetchJobGrades, createJobGrade, updateJobGrade, deleteJobGrade, toast, setToast };
};
