using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGManagement.Domain.Enums;

public enum RoomType { Single, Double, Triple, Dormitory }
public enum RoomStatus { Vacant, Occupied, Maintenance }
public enum BookingStatus { Pending, Confirmed, Cancelled, CheckedOut }
public enum PaymentType { Rent, Deposit }
public enum PaymentStatus { Pending, Paid, Overdue }
public enum AppRole { SuperAdmin, Owner, Tenant }
