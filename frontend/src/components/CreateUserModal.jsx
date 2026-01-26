import { useState, useEffect } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import userManagementApi from '../services/userManagementApi';

const CreateUserModal = ({ open, onClose, onSuccess }) => {
  const [formData, setFormData] = useState({
    loginID: '',
    password: '',
    confirmPassword: '',
    userMobileNo: '',
    userEmailID: '',
    fullName: '',
    roleID: '',
    departmentID: '',
    securityQuestionID: '',
    securityQuestionAnswer: '',
  });

  const [roles, setRoles] = useState([]);
  const [departments, setDepartments] = useState([]);
  const [securityQuestions, setSecurityQuestions] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [isDepartmentDisabled, setIsDepartmentDisabled] = useState(false);

  // Load master data
  useEffect(() => {
    if (open) {
      loadMasterData();
    }
  }, [open]);

  // Check if System Admin role is selected
  useEffect(() => {
    const selectedRole = roles.find(r => (r.roleID || r.RoleID) === parseInt(formData.roleID));
    if (selectedRole?.roleName === 'System Admin' || selectedRole?.RoleName === 'System Admin') {
      setIsDepartmentDisabled(true);
      setFormData(prev => ({ ...prev, departmentID: '' }));
    } else {
      setIsDepartmentDisabled(false);
    }
  }, [formData.roleID, roles]);

  const loadMasterData = async () => {
    try {
      const [rolesData, departmentsData, questionsData] = await Promise.all([
        userManagementApi.getAllRoles(),
        userManagementApi.getAllDepartments(),
        userManagementApi.getAllSecurityQuestions(),
      ]);

      setRoles(rolesData);
      setDepartments(departmentsData);
      setSecurityQuestions(questionsData);
    } catch (err) {
      console.error('Error loading master data:', err);
      setError('Failed to load master data. Please try again.');
    }
  };

  const handleChange = (field, value) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    setError('');
  };

  const validateMobileNumber = (mobile) => {
    const mobileRegex = /^\d{10}$/;
    return mobileRegex.test(mobile);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      // Validation
      if (formData.password !== formData.confirmPassword) {
        setError('Passwords do not match');
        setLoading(false);
        return;
      }

      if (!validateMobileNumber(formData.userMobileNo)) {
        setError('Mobile number must be exactly 10 digits');
        setLoading(false);
        return;
      }

      // Prepare data for API
      const userData = {
        loginID: formData.loginID,
        password: formData.password,
        userMobileNo: formData.userMobileNo,
        userEmailID: formData.userEmailID || null,
        fullName: formData.fullName,
        roleID: parseInt(formData.roleID),
        departmentID: formData.departmentID ? parseInt(formData.departmentID) : null,
        securityQuestionID: formData.securityQuestionID ? parseInt(formData.securityQuestionID) : null,
        securityQuestionAnswer: formData.securityQuestionAnswer || null,
      };

      await userManagementApi.createUser(userData);
      
      // Reset form
      setFormData({
        loginID: '',
        password: '',
        confirmPassword: '',
        userMobileNo: '',
        userEmailID: '',
        fullName: '',
        roleID: '',
        departmentID: '',
        securityQuestionID: '',
        securityQuestionAnswer: '',
      });

      if (onSuccess) {
        onSuccess();
      }
      onClose();
    } catch (err) {
      setError(err.message || 'Failed to create user. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  const handleClose = () => {
    setFormData({
      loginID: '',
      password: '',
      confirmPassword: '',
      userMobileNo: '',
      userEmailID: '',
      fullName: '',
      roleID: '',
      departmentID: '',
      securityQuestionID: '',
      securityQuestionAnswer: '',
    });
    setError('');
    onClose();
  };

  return (
    <Dialog open={open} onOpenChange={handleClose}>
      <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>Create New User</DialogTitle>
        </DialogHeader>

        <form onSubmit={handleSubmit} className="space-y-4">
          {error && (
            <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
              {error}
            </div>
          )}

          <div className="grid grid-cols-2 gap-4">
            <div>
              <Label htmlFor="loginID">Login ID *</Label>
              <Input
                id="loginID"
                value={formData.loginID}
                onChange={(e) => handleChange('loginID', e.target.value)}
                required
                className="mt-1"
                placeholder="Enter Login ID"
              />
            </div>

            <div>
              <Label htmlFor="fullName">Full Name *</Label>
              <Input
                id="fullName"
                value={formData.fullName}
                onChange={(e) => handleChange('fullName', e.target.value)}
                required
                className="mt-1"
                placeholder="Enter Full Name"
              />
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <Label htmlFor="password">Password *</Label>
              <Input
                id="password"
                type="password"
                value={formData.password}
                onChange={(e) => handleChange('password', e.target.value)}
                required
                minLength={6}
                className="mt-1"
                placeholder="Enter Password"
              />
            </div>

            <div>
              <Label htmlFor="confirmPassword">Confirm Password *</Label>
              <Input
                id="confirmPassword"
                type="password"
                value={formData.confirmPassword}
                onChange={(e) => handleChange('confirmPassword', e.target.value)}
                required
                className="mt-1"
                placeholder="Confirm Password"
              />
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <Label htmlFor="userMobileNo">Mobile Number * (10 digits)</Label>
              <Input
                id="userMobileNo"
                value={formData.userMobileNo}
                onChange={(e) => handleChange('userMobileNo', e.target.value.replace(/\D/g, '').slice(0, 10))}
                required
                maxLength={10}
                pattern="\d{10}"
                className="mt-1"
                placeholder="9876543210"
              />
            </div>

            <div>
              <Label htmlFor="userEmailID">Email ID</Label>
              <Input
                id="userEmailID"
                type="email"
                value={formData.userEmailID}
                onChange={(e) => handleChange('userEmailID', e.target.value)}
                className="mt-1"
                placeholder="user@example.com"
              />
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <Label htmlFor="roleID">Role *</Label>
              <Select
                value={formData.roleID}
                onValueChange={(value) => handleChange('roleID', value)}
                required
              >
                <SelectTrigger className="mt-1">
                  <SelectValue placeholder="Select Role" />
                </SelectTrigger>
                <SelectContent>
                  {roles.map((role) => (
                    <SelectItem key={role.roleID || role.RoleID} value={(role.roleID || role.RoleID).toString()}>
                      {role.roleName || role.RoleName}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div>
              <Label htmlFor="departmentID">Department</Label>
              <Select
                value={formData.departmentID}
                onValueChange={(value) => handleChange('departmentID', value)}
                disabled={isDepartmentDisabled}
              >
                <SelectTrigger className="mt-1">
                  <SelectValue placeholder={isDepartmentDisabled ? "N/A for System Admin" : "Select Department"} />
                </SelectTrigger>
                <SelectContent>
                  {departments.map((dept) => (
                    <SelectItem key={dept.departmentID || dept.DepartmentID} value={(dept.departmentID || dept.DepartmentID).toString()}>
                      {dept.departmentName || dept.DepartmentName} ({dept.departmentCode || dept.DepartmentCode})
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <Label htmlFor="securityQuestionID">Security Question</Label>
              <Select
                value={formData.securityQuestionID}
                onValueChange={(value) => handleChange('securityQuestionID', value)}
              >
                <SelectTrigger className="mt-1">
                  <SelectValue placeholder="Select Security Question" />
                </SelectTrigger>
                <SelectContent>
                  {securityQuestions.map((question) => (
                    <SelectItem key={question.securityQuestionID || question.SecurityQuestionID} value={(question.securityQuestionID || question.SecurityQuestionID).toString()}>
                      {question.questionText || question.QuestionText}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div>
              <Label htmlFor="securityQuestionAnswer">Security Answer</Label>
              <Input
                id="securityQuestionAnswer"
                value={formData.securityQuestionAnswer}
                onChange={(e) => handleChange('securityQuestionAnswer', e.target.value)}
                className="mt-1"
                placeholder="Enter Security Answer"
                disabled={!formData.securityQuestionID}
              />
            </div>
          </div>

          <DialogFooter>
            <Button type="button" variant="outline" onClick={handleClose} disabled={loading}>
              Cancel
            </Button>
            <Button type="submit" disabled={loading}>
              {loading ? 'Creating...' : 'Create User'}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
};

export default CreateUserModal;
