import 'package:figma_design/login_page.dart';
import 'package:flutter/material.dart';

class RecentActivities extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('Recent Activities', style: TextStyle(color: Colors.black)),
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
              'Recent Activities',
              style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
            ),
            SizedBox(height: 20),
            _buildActivityItem(
              'U1',
              'Student added',
              'Admin added a new student',
              '1h ago',
            ),
            _buildActivityItem(
              'U2',
              'Driver rating updated',
              'Supervisor rated a driver',
              '2h ago',
            ),
            _buildActivityItem(
              'U3',
              'Notification sent',
              'Parent notification sent',
              '3h ago',
            ),
            _buildActivityItem(
              'U4',
              'Profile updated',
              'User updated profile',
              '4h ago',
            ),
            _buildActivityItem(
              'U5',
              'Absence reported',
              'Student absence reported',
              '5h ago',
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildActivityItem(
    String userIcon,
    String activity,
    String description,
    String time,
  ) {
    return Container(
      padding: EdgeInsets.all(10.0),
      margin: EdgeInsets.symmetric(vertical: 5.0),
      decoration: BoxDecoration(
        border: Border.all(color: Colors.grey.shade300),
        borderRadius: BorderRadius.circular(10.0),
      ),
      child: Row(
        children: [
          CircleAvatar(
            backgroundColor: Colors.grey.shade300,
            child: Text(userIcon),
          ),
          SizedBox(width: 10),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(activity, style: TextStyle(fontSize: 16)),
                Text(
                  description,
                  style: TextStyle(fontSize: 12, color: Colors.grey),
                ),
              ],
            ),
          ),
          Text(time, style: TextStyle(fontSize: 12, color: Colors.grey)),
        ],
      ),
    );
  }
}
