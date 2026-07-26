import { useState, useCallback } from 'react';
import { positionApi } from '../api/positionApi';
import { PositionDto, CreatePositionCommand, UpdatePositionCommand } from '../types/position.types';

export const usePositions = () => {
  const [positions, setPositions] = useState<PositionDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [toast, setToast] = useState<{ message: string; type: "success" | "error" } | null>(null);

  const showToast = (message: string, type: "success" | "error") => {
    setToast({ message, type });
  };

  const fetchPositions = useCallback(async (searchTerm?: string, trangThai?: string, idPhongBan?: string) => {
    setLoading(true);
    try {
      const res = await positionApi.getPositions({ searchTerm, trangThai, idPhongBan });
      if (res.succeeded) setPositions(res.data);
    } catch (error) { const err = error as import('axios').AxiosError<{Message?: string}>;
      showToast(err?.response?.data?.Message || 'Lỗi tải danh sách chức vụ', 'error');
    } finally {
      setLoading(false);
    }
  }, []);

  const createPosition = async (data: CreatePositionCommand) => {
    try {
      const res = await positionApi.createPosition(data);
      if (res.succeeded) {
        showToast('Thêm mới thành công!', 'success');
        return true;
      }
    } catch (error) { const err = error as import('axios').AxiosError<{Message?: string}>;
      showToast(err?.response?.data?.Message || 'Lỗi thêm mới', 'error');
    }
    return false;
  };

  const updatePosition = async (id: string, data: UpdatePositionCommand) => {
    try {
      const res = await positionApi.updatePosition(id, data);
      if (res.succeeded) {
        showToast('Cập nhật thành công!', 'success');
        return true;
      }
    } catch (error) { const err = error as import('axios').AxiosError<{Message?: string}>;
      showToast(err?.response?.data?.Message || 'Lỗi cập nhật', 'error');
    }
    return false;
  };

  const toggleStatus = async (id: string) => {
    try {
      const res = await positionApi.toggleStatus(id);
      if (res.succeeded) {
        showToast('Đổi trạng thái thành công!', 'success');
        return true;
      }
    } catch (error) { const err = error as import('axios').AxiosError<{Message?: string}>;
      showToast(err?.response?.data?.Message || 'Lỗi chuyển trạng thái', 'error');
    }
    return false;
  };

  return { positions, loading, fetchPositions, createPosition, updatePosition, toggleStatus, toast, setToast };
};