import { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import menuApi from '../services/menuApi';
import userManagementApi from '../services/userManagementApi';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { Label } from '@/components/ui/label';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Checkbox } from '@/components/ui/checkbox';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Loader2, Save, CheckCircle2 } from 'lucide-react';

const MenuConfiguration = () => {
  const { user } = useAuth();
  const [departments, setDepartments] = useState([]);
  const [menus, setMenus] = useState([]);
  const [selectedDepartment, setSelectedDepartment] = useState('');
  const [selectedMenus, setSelectedMenus] = useState([]);
  const [departmentMenus, setDepartmentMenus] = useState([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  useEffect(() => {
    loadData();
  }, []);

  useEffect(() => {
    if (selectedDepartment) {
      loadDepartmentMenus(parseInt(selectedDepartment));
    }
  }, [selectedDepartment]);

  const loadData = async () => {
    try {
      setLoading(true);
      setError('');
      
      // Load departments
      const depts = await userManagementApi.getAllDepartments();
      setDepartments(depts);

      // Load all menus
      const allMenus = await menuApi.getAllMenus();
      // Filter out admin-only menus if not admin
      const filteredMenus = allMenus.filter(m => {
        const isAdmin = user?.roles?.includes('System Admin') || user?.RoleName === 'System Admin';
        return !m.isAdminOnly || isAdmin;
      });
      setMenus(filteredMenus);
    } catch (err) {
      setError('Failed to load data: ' + err.message);
    } finally {
      setLoading(false);
    }
  };

  const loadDepartmentMenus = async (departmentId) => {
    try {
      const deptMenus = await menuApi.getDepartmentMenus(departmentId);
      const menuIds = deptMenus.map(m => m.menuID);
      setDepartmentMenus(menuIds);
      setSelectedMenus([...menuIds]);
    } catch (err) {
      setError('Failed to load department menus: ' + err.message);
    }
  };

  const handleMenuToggle = (menuId) => {
    setSelectedMenus(prev => {
      if (prev.includes(menuId)) {
        return prev.filter(id => id !== menuId);
      } else {
        return [...prev, menuId];
      }
    });
  };

  const handleSave = async () => {
    if (!selectedDepartment) {
      setError('Please select a department');
      return;
    }

    try {
      setSaving(true);
      setError('');
      setSuccess('');

      await menuApi.assignMenusToDepartment(parseInt(selectedDepartment), selectedMenus);
      
      setSuccess('Menus assigned to department successfully!');
      setDepartmentMenus([...selectedMenus]);
      
      // Clear success message after 3 seconds
      setTimeout(() => setSuccess(''), 3000);
    } catch (err) {
      setError('Failed to save: ' + err.message);
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
      </div>
    );
  }

  return (
    <div className="container mx-auto py-8 px-4 max-w-6xl">
      <div className="mb-6">
        <h1 className="text-3xl font-bold mb-2">Menu Configuration</h1>
        <p className="text-muted-foreground">
          Assign menus to departments. Users in a department will only see menus assigned to their department.
        </p>
      </div>

      {error && (
        <Alert variant="destructive" className="mb-4">
          <AlertDescription>{error}</AlertDescription>
        </Alert>
      )}

      {success && (
        <Alert className="mb-4 bg-green-50 border-green-200">
          <CheckCircle2 className="h-4 w-4 text-green-600" />
          <AlertDescription className="text-green-800">{success}</AlertDescription>
        </Alert>
      )}

      <Card className="p-6">
        <div className="space-y-6">
          <div>
            <Label htmlFor="department">Select Department</Label>
            <Select
              value={selectedDepartment}
              onValueChange={setSelectedDepartment}
            >
              <SelectTrigger id="department" className="mt-2">
                <SelectValue placeholder="Select a department" />
              </SelectTrigger>
              <SelectContent>
                {departments.map((dept) => (
                  <SelectItem key={dept.departmentID} value={dept.departmentID.toString()}>
                    {dept.departmentName} ({dept.departmentCode})
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          {selectedDepartment && (
            <>
              <div>
                <Label className="mb-4 block">Select Menus for Department</Label>
                <div className="space-y-3 border rounded-lg p-4 max-h-96 overflow-y-auto">
                  {menus
                    .filter(m => !m.isAdminOnly) // Don't show admin-only menus in assignment
                    .sort((a, b) => a.displayOrder - b.displayOrder)
                    .map((menu) => (
                      <div key={menu.menuID} className="flex items-center space-x-2">
                        <Checkbox
                          id={`menu-${menu.menuID}`}
                          checked={selectedMenus.includes(menu.menuID)}
                          onCheckedChange={() => handleMenuToggle(menu.menuID)}
                        />
                        <Label
                          htmlFor={`menu-${menu.menuID}`}
                          className="flex items-center gap-2 cursor-pointer flex-1"
                        >
                          <span className="font-medium">{menu.menuName}</span>
                          <span className="text-sm text-muted-foreground">({menu.menuPath})</span>
                          {menu.menuDescription && (
                            <span className="text-xs text-muted-foreground">- {menu.menuDescription}</span>
                          )}
                        </Label>
                      </div>
                    ))}
                </div>
              </div>

              <div className="flex justify-end gap-4">
                <Button
                  variant="outline"
                  onClick={() => setSelectedMenus([...departmentMenus])}
                  disabled={saving}
                >
                  Reset
                </Button>
                <Button onClick={handleSave} disabled={saving}>
                  {saving ? (
                    <>
                      <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                      Saving...
                    </>
                  ) : (
                    <>
                      <Save className="mr-2 h-4 w-4" />
                      Save Configuration
                    </>
                  )}
                </Button>
              </div>
            </>
          )}
        </div>
      </Card>

      <Card className="p-6 mt-6">
        <h2 className="text-xl font-semibold mb-4">Menu Access Rules</h2>
        <ul className="space-y-2 text-sm text-muted-foreground">
          <li>• <strong>System Admin:</strong> Has access to all menus including admin-only menus</li>
          <li>• <strong>DESA Head:</strong> Has access to all department menus (non-admin menus only)</li>
          <li>• <strong>Department Users:</strong> Can only access menus assigned to their department</li>
          <li>• <strong>Dashboard:</strong> Accessible to all users by default</li>
        </ul>
      </Card>
    </div>
  );
};

export default MenuConfiguration;
