import { useState, useCallback } from 'react';
import { message } from 'antd';
import { positionApi } from '../api/positionApi';
import { PositionDto, CreatePositionCommand, UpdatePositionCommand } from '../types/position.types';

export const usePositions = () => {
  const [positions, setPositions] = useState<PositionDto[]>([]);
  const [loading, setLoading] = useState(false);

  const fetchPositions = useCallback(async (searchTerm?: string, trangThai?: string, idPhongBan?: string) => {
    setLoading(true);
    try {
      const res = await positionApi.getPositions({ searchTerm, trangThai, idPhongBan });
      if (res.succeeded) setPositions(res.data);
    } catch (error) { const err = error as import('axios').AxiosError<{Message?: string}>;
      message.error(err?.response?.data?.Message || 'Lỗi tải danh sách chức vụ');
    } finally {
      setLoading(false);
    }
  }, []);

  const createPosition = async (data: CreatePositionCommand) => {
    try {
      const res = await positionApi.createPosition(data);
      if (res.succeeded) {
        message.success('Thêm mới thành công!');
        return true;
      }
    } catch (error) { const err = error as import('axios').AxiosError<{Message?: string}>;
      message.error(err?.response?.data?.Message || 'Lỗi thêm mới');
    }
    return false;
  };

  const updatePosition = async (id: string, data: UpdatePositionCommand) => {
    try {
      const res = await positionApi.updatePosition(id, data);
      if (res.succeeded) {
        message.success('Cập nhật thành công!');
        return true;
      }
    } catch (error) { const err = error as import('axios').AxiosError<{Message?: string}>;
      message.error(err?.response?.data?.Message || 'Lỗi cập nhật');
    }
    return false;
  };

  const toggleStatus = async (id: string) => {
    try {
      const res = await positionApi.toggleStatus(id);
      if (res.succeeded) {
        message.success('Đổi trạng thái thành công!');
        return true;
      }
    } catch (error) { const err = error as import('axios').AxiosError<{Message?: string}>;
      message.error(err?.response?.data?.Message || 'Lỗi chuyển trạng thái');
    }
    return false;
  };

  return { positions, loading, fetchPositions, createPosition, updatePosition, toggleStatus };
};