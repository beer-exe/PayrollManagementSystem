import { useState, useCallback } from 'react';
import { message } from 'antd';
import { jobGradeApi } from '../api/jobGradeApi';
import { JobGrade, CreateJobGradeDto, UpdateJobGradeDto } from '../types/jobGrade.types';

export const useJobGrades = () => {
  const [jobGrades, setJobGrades] = useState<JobGrade[]>([]);
  const [loading, setLoading] = useState(false);

  const fetchJobGrades = useCallback(async () => {
    setLoading(true);
    try {
      const res = await jobGradeApi.getJobGrades();
      if (res.succeeded) setJobGrades(res.data);
    } catch (error) { const err = error as import('axios').AxiosError<{Message?: string}>;
      message.error(err?.response?.data?.Message || 'Lỗi tải danh sách ngạch lương');
    } finally {
      setLoading(false);
    }
  }, []);

  const createJobGrade = async (data: CreateJobGradeDto) => {
    try {
      const res = await jobGradeApi.createJobGrade(data);
      if (res.succeeded) {
        message.success('Thêm mới thành công!');
        return true;
      }
    } catch (error) { const err = error as import('axios').AxiosError<{Message?: string}>;
      message.error(err?.response?.data?.Message || 'Lỗi thêm mới');
    }
    return false;
  };

  const updateJobGrade = async (data: UpdateJobGradeDto) => {
    try {
      const res = await jobGradeApi.updateJobGrade(data);
      if (res.succeeded) {
        message.success('Cập nhật thành công!');
        return true;
      }
    } catch (error) { const err = error as import('axios').AxiosError<{Message?: string}>;
      message.error(err?.response?.data?.Message || 'Lỗi cập nhật');
    }
    return false;
  };

  const deleteJobGrade = async (id: string) => {
    try {
      const res = await jobGradeApi.deleteJobGrade(id);
      if (res.succeeded) {
        message.success('Xóa thành công!');
        return true;
      }
    } catch (error) { const err = error as import('axios').AxiosError<{Message?: string}>;
      message.error(err?.response?.data?.Message || 'Lỗi xóa');
    }
    return false;
  };

  return { jobGrades, loading, fetchJobGrades, createJobGrade, updateJobGrade, deleteJobGrade };
};
