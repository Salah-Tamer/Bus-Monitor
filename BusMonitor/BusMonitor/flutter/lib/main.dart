import 'package:flutter/material.dart';
import 'package:figma_design/login_page.dart';
import 'package:figma_design/student_status.dart';
import 'package:figma_design/behavior_reports.dart';
import 'package:figma_design/delay_notifications.dart';
import 'package:figma_design/home_page.dart';
import 'package:figma_design/admin_panel.dart';
import 'package:figma_design/recent_activities.dart';
import 'package:figma_design/dashboard_overview.dart';
import 'package:figma_design/admin_dashboard.dart';
import 'package:figma_design/driver_dashboard.dart';
import 'package:figma_design/student_list.dart';
import 'package:figma_design/driver_rating.dart';
import 'package:figma_design/driver_trips.dart';

void main() {
  runApp(MyApp());
}

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Bus Monitor',
      theme: ThemeData(
        primarySwatch: MaterialColor(0xFFFFFFFF, <int, Color>{
          50: Color(0xFFFFFFFF),
          100: Color(0xFFFFFFFF),
          200: Color(0xFFFFFFFF),
          300: Color(0xFFFFFFFF),
          400: Color(0xFFFFFFFF),
          500: Color(0xFFFFFFFF),
          600: Color(0xFFFFFFFF),
          700: Color(0xFFFFFFFF),
          800: Color(0xFFFFFFFF),
          900: Color(0xFFFFFFFF),
        }),
        scaffoldBackgroundColor: Colors.white,
      ),
      home: LoginPage(),
    );
  }
}

class SupervisorDashboard extends StatefulWidget {
  @override
  _SupervisorDashboardState createState() => _SupervisorDashboardState();
}

class _SupervisorDashboardState extends State<SupervisorDashboard> {
  int _selectedIndex = 0;
  static const TextStyle optionStyle =
      TextStyle(fontSize: 30, fontWeight: FontWeight.bold);
  static  List<Widget> _widgetOptions =  [
    HomePage(),
    StudentStatus(),
    BehaviorReportsPage(),
    DelayNotificationsPage(),
  ];

  void _onItemTapped(int index) {
    setState(() {
      _selectedIndex = index;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text(
          'Supervisor Dashboard',
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
      body: Center(
        child: _widgetOptions.elementAt(_selectedIndex),
      ),
      bottomNavigationBar: BottomNavigationBar(
        items:  [
          BottomNavigationBarItem(
            icon: Icon(Icons.home),
            label: 'Home',
          ),
          BottomNavigationBarItem(
            icon: Icon(Icons.school),
            label: 'Student Status',
          ),
          BottomNavigationBarItem(
            icon: Icon(Icons.report),
            label: 'Behavior Reports',
          ),
          BottomNavigationBarItem(
            icon: Icon(Icons.notifications),
            label: 'Delay Notifications',
          ),
        ],
        currentIndex: _selectedIndex,
        selectedItemColor: Colors.blue[800],
        unselectedItemColor: const Color.fromARGB(255, 55, 55, 56),
        onTap: _onItemTapped,
      ),
    );
  }
}
