import React, { useState, useEffect } from 'react';
import './ShiftAssignment.css';
import { shiftAssignmentApi } from '../api/shiftAssignmentApi';
import { PhanCongCaDto } from '../types';
import { employeeApi } from '../../employees/api/employeeApi';
import { Employee } from '../../employees/types';
import { workShiftApi } from '../../workShifts/api/workShiftApi';
import { CaLamViec } from '../../workShifts/types';

// Helper to get current week's Monday and Sunday
const getWeekDates = (date: Date) => {
    const d = new Date(date);
    const day = d.getDay(), diff = d.getDate() - day + (day === 0 ? -6 : 1);
    const monday = new Date(d.setDate(diff));
    const dates = [];
    for (let i = 0; i < 7; i++) {
        const nextDate = new Date(monday);
        nextDate.setDate(monday.getDate() + i);
        dates.push(nextDate);
    }
    return dates;
};

const formatDateToYYYYMMDD = (date: Date) => {
    const y = date.getFullYear();
    const m = String(date.getMonth() + 1).padStart(2, '0');
    const d = String(date.getDate()).padStart(2, '0');
    return `${y}-${m}-${d}`;
};

const formatDisplayDate = (date: Date) => {
    const days = ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'];
    return `${days[date.getDay()]} - ${date.getDate()}/${date.getMonth() + 1}`;
};

// Generate a random but consistent color for a string (for shift chips)
const getShiftColor = (shiftId: string) => {
    let hash = 0;
    for (let i = 0; i < shiftId.length; i++) {
        hash = shiftId.charCodeAt(i) + ((hash << 5) - hash);
    }
    const h = Math.abs(hash) % 360;
    return {
        bg: `hsl(${h}, 70%, 90%)`,
        border: `hsl(${h}, 70%, 75%)`,
        text: `hsl(${h}, 80%, 30%)`
    };
};

export const ShiftAssignment: React.FC = () => {
    const [currentDate, setCurrentDate] = useState(new Date());
    const [weekDates, setWeekDates] = useState<Date[]>([]);
    
    const [employees, setEmployees] = useState<Employee[]>([]);
    const [shifts, setShifts] = useState<CaLamViec[]>([]);
    const [assignments, setAssignments] = useState<PhanCongCaDto[]>([]);
    
    const [isLoading, setIsLoading] = useState(false);
    const [toast, setToast] = useState<{ message: string; type: "success" | "error" } | null>(null);

    const [draggedShiftId, setDraggedShiftId] = useState<string | null>(null);

    useEffect(() => {
        setWeekDates(getWeekDates(currentDate));
    }, [currentDate]);

    useEffect(() => {
        const fetchInitialData = async () => {
            try {
                const [empRes, shiftRes] = await Promise.all([
                    employeeApi.getAll(),
                    workShiftApi.getAll()
                ]);
                if (empRes.succeeded) setEmployees(empRes.data.filter(e => e.trangThai === 0)); // Only active
                if (shiftRes.succeeded) setShifts(shiftRes.data.filter(s => s.trangThai)); // Only active shifts
            } catch (err) {
                console.error(err);
            }
        };
        fetchInitialData();
    }, []);

    useEffect(() => {
        if (weekDates.length > 0) {
            loadAssignments();
        }
    }, [weekDates]);

    const loadAssignments = async () => {
        setIsLoading(true);
        try {
            const startDate = formatDateToYYYYMMDD(weekDates[0]);
            const endDate = formatDateToYYYYMMDD(weekDates[6]);
            const res = await shiftAssignmentApi.getByDateRange(startDate, endDate);
            if (res.succeeded) {
                setAssignments(res.data);
            }
        } catch (error) {
            setToast({ message: "Lỗi khi tải dữ liệu phân công ca", type: "error" });
        } finally {
            setIsLoading(false);
        }
    };

    const handlePrevWeek = () => {
        const d = new Date(currentDate);
        d.setDate(d.getDate() - 7);
        setCurrentDate(d);
    };

    const handleNextWeek = () => {
        const d = new Date(currentDate);
        d.setDate(d.getDate() + 7);
        setCurrentDate(d);
    };

    // Drag & Drop Handlers
    const handleDragStart = (e: React.DragEvent, shiftId: string) => {
        setDraggedShiftId(shiftId);
        e.dataTransfer.effectAllowed = 'copyMove';
        // Hide the ghost image a bit if possible, or just let default behavior
    };

    const handleDragOver = (e: React.DragEvent) => {
        e.preventDefault(); // Necessary to allow dropping
        e.dataTransfer.dropEffect = 'copy';
        e.currentTarget.classList.add('drag-over');
    };

    const handleDragLeave = (e: React.DragEvent) => {
        e.currentTarget.classList.remove('drag-over');
    };

    const handleDrop = async (e: React.DragEvent, cccdNhanVien: string, date: Date) => {
        e.preventDefault();
        e.currentTarget.classList.remove('drag-over');

        if (!draggedShiftId) return;

        const dateStr = formatDateToYYYYMMDD(date);
        
        // Optimistic UI Update
        const shiftObj = shifts.find(s => s.id === draggedShiftId);
        const existingIndex = assignments.findIndex(a => a.cccdNhanVien === cccdNhanVien && a.ngayLamViec === dateStr);
        
        let newAssignments = [...assignments];
        if (existingIndex >= 0) {
            newAssignments[existingIndex] = { ...newAssignments[existingIndex], idCaLamViec: draggedShiftId, tenCa: shiftObj?.tenCa || '' };
        } else {
            newAssignments.push({
                idPhanCong: 'temp',
                cccdNhanVien,
                ngayLamViec: dateStr,
                idCaLamViec: draggedShiftId,
                tenCa: shiftObj?.tenCa || '',
                hoTenNhanVien: ''
            });
        }
        setAssignments(newAssignments);

        // API Call
        try {
            await shiftAssignmentApi.upsert({
                cccdNhanVien,
                ngayLamViec: dateStr,
                idCaLamViec: draggedShiftId
            });
        } catch (error) {
            setToast({ message: "Lưu phân công thất bại", type: "error" });
            loadAssignments(); // Rollback
        }
        
        setDraggedShiftId(null);
    };

    const handleRemoveAssignment = async (cccdNhanVien: string, date: Date) => {
        const dateStr = formatDateToYYYYMMDD(date);
        
        // Optimistic UI Update
        setAssignments(prev => prev.filter(a => !(a.cccdNhanVien === cccdNhanVien && a.ngayLamViec === dateStr)));

        try {
            await shiftAssignmentApi.upsert({
                cccdNhanVien,
                ngayLamViec: dateStr,
                idCaLamViec: null
            });
        } catch (error) {
            setToast({ message: "Xoá phân công thất bại", type: "error" });
            loadAssignments(); // Rollback
        }
    };

    return (
        <div className="sa-container">
            <div className="sa-header">
                <div className="sa-header-left">
                    <h2>Phân công ca luân phiên</h2>
                    <p>Sắp xếp và quản lý ca làm việc cho nhân viên bằng thao tác kéo thả (Drag & Drop)</p>
                </div>
            </div>

            <div className="sa-toolbar">
                <div className="sa-date-selector">
                    <button className="sa-btn-icon" onClick={handlePrevWeek}>
                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{width: 18}}>
                            <path strokeLinecap="round" strokeLinejoin="round" d="M15.75 19.5L8.25 12l7.5-7.5" />
                        </svg>
                    </button>
                    <div className="sa-current-date">
                        {weekDates.length > 0 && `${formatDisplayDate(weekDates[0])} - ${formatDisplayDate(weekDates[6])}`}
                    </div>
                    <button className="sa-btn-icon" onClick={handleNextWeek}>
                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{width: 18}}>
                            <path strokeLinecap="round" strokeLinejoin="round" d="M8.25 4.5l7.5 7.5-7.5 7.5" />
                        </svg>
                    </button>
                </div>

                <div className="sa-shifts-palette">
                    <span className="sa-palette-label">Kéo thả ca:</span>
                    {shifts.map(s => {
                        const colors = getShiftColor(s.id);
                        return (
                            <div 
                                key={s.id}
                                className="sa-shift-chip"
                                style={{ backgroundColor: colors.bg, borderColor: colors.border, color: colors.text }}
                                draggable
                                onDragStart={(e) => handleDragStart(e, s.id)}
                            >
                                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" style={{ width: 16 }}>
                                    <path fillRule="evenodd" d="M12 2.25c-5.385 0-9.75 4.365-9.75 9.75s4.365 9.75 9.75 9.75 9.75-4.365 9.75-9.75S17.385 2.25 12 2.25ZM12.75 6a.75.75 0 0 0-1.5 0v6c0 .414.336.75.75.75h4.5a.75.75 0 0 0 0-1.5h-3.75V6Z" clipRule="evenodd" />
                                </svg>
                                {s.tenCa}
                            </div>
                        )
                    })}
                </div>
            </div>

            <div className="sa-matrix-container" style={{ position: 'relative' }}>
                {isLoading && (
                    <div className="sa-loading-overlay">
                        Đang tải dữ liệu...
                    </div>
                )}
                <table className="sa-matrix-table">
                    <thead>
                        <tr>
                            <th>Nhân viên</th>
                            {weekDates.map((date, idx) => (
                                <th key={idx}>{formatDisplayDate(date)}</th>
                            ))}
                        </tr>
                    </thead>
                    <tbody>
                        {employees.map(emp => (
                            <tr key={emp.cccd}>
                                <td>
                                    <div className="sa-emp-info">
                                        <span className="sa-emp-name">{emp.hoTen}</span>
                                        <span className="sa-emp-dept">{emp.phongBan?.tenPhongBan || 'Chưa xếp phòng'}</span>
                                    </div>
                                </td>
                                {weekDates.map((date, idx) => {
                                    const dateStr = formatDateToYYYYMMDD(date);
                                    const assignment = assignments.find(a => a.cccdNhanVien === emp.cccd && a.ngayLamViec === dateStr);
                                    
                                    return (
                                        <td key={idx}>
                                            <div 
                                                className="sa-drop-cell"
                                                onDragOver={handleDragOver}
                                                onDragLeave={handleDragLeave}
                                                onDrop={(e) => handleDrop(e, emp.cccd, date)}
                                            >
                                                {assignment && (
                                                    <div 
                                                        className="sa-assigned-shift"
                                                        style={{ 
                                                            backgroundColor: getShiftColor(assignment.idCaLamViec).bg, 
                                                            border: `1px solid ${getShiftColor(assignment.idCaLamViec).border}`,
                                                            color: getShiftColor(assignment.idCaLamViec).text
                                                        }}
                                                        draggable
                                                        onDragStart={(e) => handleDragStart(e, assignment.idCaLamViec)}
                                                    >
                                                        {assignment.tenCa}
                                                        <div 
                                                            className="sa-btn-remove-shift"
                                                            onClick={(e) => {
                                                                e.stopPropagation();
                                                                handleRemoveAssignment(emp.cccd, date);
                                                            }}
                                                        >
                                                            ✕
                                                        </div>
                                                    </div>
                                                )}
                                            </div>
                                        </td>
                                    );
                                })}
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>

            {toast && (
                <div style={{ position: 'fixed', bottom: '20px', right: '20px', padding: '1rem', background: toast.type === 'success' ? 'var(--success-bg)' : 'var(--danger-bg)', color: toast.type === 'success' ? 'var(--success-text)' : 'var(--danger-text)', borderRadius: '8px', zIndex: 9999, fontWeight: 500, boxShadow: '0 4px 12px rgba(0,0,0,0.1)' }}>
                    {toast.message}
                    <button onClick={() => setToast(null)} style={{ background: 'transparent', border: 'none', marginLeft: '10px', cursor: 'pointer', color: 'inherit' }}>✕</button>
                </div>
            )}
        </div>
    );
};
