import { useState, useEffect } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
// Using native div with overflow for now
import { Clock, User, MessageSquare } from 'lucide-react';
import workflowApi from '@/services/workflowApi';

const AuditHistoryModal = ({ open, onOpenChange, tableName, recordId }) => {
  const [history, setHistory] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (open && tableName && recordId) {
      loadHistory();
    }
  }, [open, tableName, recordId]);

  const loadHistory = async () => {
    try {
      setLoading(true);
      setError('');
      const data = await workflowApi.getAuditHistory(tableName, recordId);
      setHistory(data);
    } catch (err) {
      setError(err.message || 'Failed to load audit history');
      console.error('Error loading audit history:', err);
    } finally {
      setLoading(false);
    }
  };

  const formatDate = (dateString) => {
    if (!dateString) return '-';
    try {
      // Parse the date string (already in IST from backend)
      const date = new Date(dateString);
      
      // Format date directly (no conversion needed as it's already in IST)
      return date.toLocaleString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
        hour12: true
      });
    } catch {
      return dateString;
    }
  };

  const getStatusColor = (statusId) => {
    switch (statusId) {
      case 1: return 'text-gray-600';
      case 2: return 'text-yellow-600';
      case 3: return 'text-red-600';
      case 4: return 'text-blue-600';
      case 5: return 'text-red-600';
      case 6: return 'text-green-600';
      default: return 'text-gray-600';
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-3xl max-h-[80vh]">
        <DialogHeader>
          <DialogTitle>Workflow Audit History</DialogTitle>
        </DialogHeader>

        {loading ? (
          <div className="flex items-center justify-center py-8">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900"></div>
          </div>
        ) : error ? (
          <div className="text-red-600 py-4">{error}</div>
        ) : history.length === 0 ? (
          <div className="text-gray-500 py-4 text-center">No audit history available</div>
        ) : (
          <style dangerouslySetInnerHTML={{__html: `
            .audit-history-timeline-scroll::-webkit-scrollbar {
              width: 8px;
              height: 8px;
            }
            .audit-history-timeline-scroll::-webkit-scrollbar-track {
              background: #f1f5f9;
              border-radius: 4px;
            }
            .audit-history-timeline-scroll::-webkit-scrollbar-thumb {
              background: #cbd5e1;
              border-radius: 4px;
            }
            .audit-history-timeline-scroll::-webkit-scrollbar-thumb:hover {
              background: #94a3b8;
            }
          `}} />
          <div className="audit-history-timeline-scroll h-[60vh] overflow-y-auto overflow-x-auto pr-4" style={{ scrollbarWidth: 'thin', scrollbarColor: '#cbd5e1 #f1f5f9' }}>
            <div className="space-y-4">
              {history.map((item, index) => (
                <div key={item.auditID} className="border-l-2 border-gray-200 pl-4 pb-4 relative">
                  {/* Timeline dot */}
                  <div className="absolute -left-2 top-0 w-4 h-4 bg-blue-600 rounded-full border-2 border-white"></div>
                  
                  <div className="space-y-2">
                    {/* Header */}
                    <div className="flex items-center justify-between">
                      <div className="flex items-center gap-2">
                        <User className="h-4 w-4 text-gray-500" />
                        <span className="font-medium">{item.actionByUserName}</span>
                      </div>
                      <div className="flex items-center gap-2 text-sm text-gray-500">
                        <Clock className="h-4 w-4" />
                        <span>{formatDate(item.actionDate)}</span>
                      </div>
                    </div>

                    {/* Action */}
                    <div className="font-semibold text-lg">{item.actionName}</div>

                    {/* Status Change */}
                    {item.fromStatusName && (
                      <div className="flex items-center gap-2 text-sm">
                        <span className={getStatusColor(item.fromStatusID)}>
                          {item.fromStatusName}
                        </span>
                        <span className="text-gray-400">→</span>
                        <span className={getStatusColor(item.toStatusID)}>
                          {item.toStatusName}
                        </span>
                      </div>
                    )}

                    {/* Remarks */}
                    {item.remarks && (
                      <div className="bg-gray-50 p-3 rounded-md border border-gray-200">
                        <div className="flex items-start gap-2">
                          <MessageSquare className="h-4 w-4 text-gray-500 mt-0.5" />
                          <div>
                            <div className="text-sm font-medium text-gray-700 mb-1">Remarks:</div>
                            <div className="text-sm text-gray-600">{item.remarks}</div>
                          </div>
                        </div>
                      </div>
                    )}
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}
      </DialogContent>
    </Dialog>
  );
};

export default AuditHistoryModal;
