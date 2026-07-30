import React, { useEffect, useRef } from 'react';
import './WorkShiftManagement.css'; // Reuse existing CSS

interface WorkShiftInstructionModalProps {
    isOpen: boolean;
    onClose: () => void;
}

export const WorkShiftInstructionModal: React.FC<WorkShiftInstructionModalProps> = ({ isOpen, onClose }) => {
    const modalRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        const handleKeyDown = (e: KeyboardEvent) => {
            if (e.key === 'Escape') onClose();
        };

        if (isOpen) {
            document.addEventListener('keydown', handleKeyDown);
            document.body.style.overflow = 'hidden';
        }

        return () => {
            document.removeEventListener('keydown', handleKeyDown);
            document.body.style.overflow = 'unset';
        };
    }, [isOpen, onClose]);

    const handleBackdropClick = (e: React.MouseEvent<HTMLDivElement>) => {
        if (e.target === e.currentTarget) {
            onClose();
        }
    };

    if (!isOpen) return null;

    return (
        <div className="wsh-modal-overlay" onClick={handleBackdropClick}>
            <div className="wsh-modal-content" ref={modalRef} style={{ maxWidth: '600px' }}>
                <div className="wsh-modal-header">
                    <h3 className="wsh-modal-title">
                        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" style={{ width: '24px', height: '24px', marginRight: '8px', color: 'var(--primary-color)' }}>
                            <path fillRule="evenodd" d="M2.25 12c0-5.385 4.365-9.75 9.75-9.75s9.75 4.365 9.75 9.75-4.365 9.75-9.75 9.75S2.25 17.385 2.25 12Zm8.706-1.442c1.146-.573 2.437.463 2.126 1.706l-.709 2.836.042-.02a.75.75 0 0 1 .67 1.34l-.04.022c-1.147.573-2.438-.463-2.127-1.706l.71-2.836-.042.02a.75.75 0 1 1-.671-1.34l.041-.022ZM12 9a.75.75 0 1 0 0-1.5.75.75 0 0 0 0 1.5Z" clipRule="evenodd" />
                        </svg>
                        Hướng dẫn cấu hình Ca làm việc
                    </h3>
                    <button className="wsh-btn-close" onClick={onClose}>
                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
                            <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
                        </svg>
                    </button>
                </div>
                <div className="wsh-modal-body" style={{ maxHeight: '60vh', overflowY: 'auto' }}>
                    <div style={{ display: 'flex', flexDirection: 'column', gap: '1.5rem', color: 'var(--text-primary)', fontSize: '0.95rem', lineHeight: '1.6' }}>

                        <div>
                            <h4 style={{ color: 'var(--primary-color)', marginBottom: '0.5rem', fontWeight: 600 }}>1. Bản chất của Ca làm việc</h4>
                            <p>Ca làm việc là dữ liệu nền tảng, để hệ thống tính toán giờ công và áp dụng cho Lịch làm việc.</p>
                        </div>

                        <div>
                            <h4 style={{ color: 'var(--primary-color)', marginBottom: '0.5rem', fontWeight: 600 }}>2. Lưu ý khi Cập nhật (Sửa)</h4>
                            <p>Hệ thống ưu tiên bảo vệ tính toàn vẹn của dữ liệu lịch sử chấm công:</p>
                            <ul style={{ paddingLeft: '1.5rem', marginTop: '0.5rem', display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
                                <li><strong>Ca chưa từng sử dụng:</strong> Bạn có thể sửa toàn bộ thông tin thoải mái.</li>
                                <li><strong>Ca đã có lịch sử:</strong> Nếu ca làm việc đã được gắn vào các ngày làm việc trong quá khứ, bạn <strong>KHÔNG THỂ</strong> thay đổi các thông tin cốt lõi làm sai lệch báo cáo cũ (như Giờ bắt đầu/kết thúc, Hệ số lương, Khung giờ nghỉ). Bạn chỉ có thể sửa Tên ca hoặc Trạng thái.</li>
                            </ul>
                            <div style={{ marginTop: '0.75rem', padding: '0.75rem', backgroundColor: 'var(--bg-secondary)', borderLeft: '4px solid var(--primary-color)', borderRadius: '4px' }}>
                                <strong>💡 Lời khuyên:</strong> Nếu muốn thay đổi giờ giấc của một ca cũ, hãy <strong>Vô hiệu hoá</strong> ca cũ đó, và <strong>Tạo một ca mới</strong> với thiết lập mới.
                            </div>
                        </div>

                        <div>
                            <h4 style={{ color: 'var(--primary-color)', marginBottom: '0.5rem', fontWeight: 600 }}>3. Lưu ý khi Xoá</h4>
                            <ul style={{ paddingLeft: '1.5rem', display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
                                <li>Chỉ có thể xoá những Ca làm việc <strong>chưa từng được gán</strong> cho bất kỳ Lịch làm việc hay Phân công ca nào.</li>
                                <li>Nếu Ca đã được sử dụng (dù là ở quá khứ hay tương lai), hệ thống sẽ chặn thao tác Xoá để tránh làm lỗi dữ liệu. Thay vào đó, hãy <strong>Vô hiệu hoá (Tắt trạng thái Hoạt động)</strong>.</li>
                            </ul>
                        </div>

                        <div>
                            <h4 style={{ color: 'var(--primary-color)', marginBottom: '0.5rem', fontWeight: 600 }}>4. Ca xuyên ngày</h4>
                            <p>Đánh dấu tick vào "Ca xuyên ngày" nếu giờ kết thúc của ca làm việc rơi vào ngày hôm sau (ví dụ: Ca đêm từ 22:00 hôm nay đến 06:00 sáng mai). Hệ thống sẽ tự động cộng thêm 24h để tính toán thời gian làm việc chuẩn xác.</p>
                        </div>

                    </div>
                </div>
                <div className="wsh-modal-footer">
                    <button className="wsh-btn-create" onClick={onClose}>
                        Đã hiểu
                    </button>
                </div>
            </div>
        </div>
    );
};
