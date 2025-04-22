import 'package:figma_design/login_page.dart';
import 'package:flutter/material.dart';

class DashboardOverview extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(
          'Dashboard Overview',
          style: TextStyle(color: Colors.black),
        ),
        backgroundColor: Colors.white,
        centerTitle: true,
        elevation: 0,
        actions: [
          IconButton
          (
            icon: const Icon(Icons.notifications_none, color: Colors.black),
            onPressed: () {},
          ),
           PopupMenuButton<String>(
              icon: const Icon(Icons.settings, color: Colors.black),
              onSelected: (String item) {
                if (item == 'Logout') {
                  Navigator.pushReplacement(
                    context,
                    MaterialPageRoute(builder: (context) => LoginPage()),
                  );
                }
              },
              itemBuilder: (BuildContext context) {
                return [
                  const PopupMenuItem<String>(
                    value: 'Settings',
                    child: Text('Settings'),
                  ),
                  const PopupMenuItem<String>(
                    value: 'Logout',
                    child: Text('Logout'),
                  ),
                ];
              },
            ),
        ],
      ),
      body: Container(
        padding: EdgeInsets.all(20.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Dashboard Overview',
              style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
            ),
            SizedBox(height: 20),
            _buildDashboardItem(
              'Total Users',
              '245',
              '+12 from last month',
              Icons.person,
            ),
            _buildDashboardItem(
              'Students',
              '178',
              '+5 from last month',
              Icons.school,
            ),
            _buildDashboardItem(
              'Drivers',
              '32',
              '+2 from last month',
              Icons.drive_eta,
            ),
            _buildDashboardItem(
              'Supervisors',
              '35',
              '+3 from last month',
              Icons.supervisor_account,
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildDashboardItem(
    String title,
    String value,
    String subtitle,
    IconData icon,
  ) {
    return Container(
      padding: EdgeInsets.all(10.0),
      margin: EdgeInsets.symmetric(vertical: 5.0),
      decoration: BoxDecoration(
        border: Border.all(color: Colors.grey.shade300),
        borderRadius: BorderRadius.circular(10.0),
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(title, style: TextStyle(fontSize: 16)),
              Text(
                value,
                style: TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
              ),
              Text(
                subtitle,
                style: TextStyle(fontSize: 12, color: Colors.grey),
              ),
            ],
          ),
          Icon(icon, color: Colors.grey),
        ],
      ),
    );
  }
}
