import { BaseApiService } from '../BaseApiService';

class Table7_7PrisonerMaintenanceExpenditureApiService extends BaseApiService {
  constructor() {
    super('/v1/SSD/Table7_7/PrisonerMaintenanceExpenditure');
  }
  async getByYear(year) {
    return this.request(`/year/${encodeURIComponent(year)}`);
  }
}

export default new Table7_7PrisonerMaintenanceExpenditureApiService();
