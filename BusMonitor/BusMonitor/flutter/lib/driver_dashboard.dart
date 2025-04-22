import 'package:figma_design/login_page.dart';
import 'package:flutter/material.dart';

class DriverDashboard extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        backgroundColor: Colors.white,
        title: Text('Driver Dashboard'),
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
          // IconButton(
          //   icon: Icon(Icons.notifications_none),
          //   onPressed: () {},
          // ),
          // IconButton(
          //   icon: Icon(Icons.settings),
          //   onPressed: () {},
          // ),
        ],
      ),
      body: Column(
        children: [
          // Row
          // (
          //   mainAxisAlignment: MainAxisAlignment.spaceAround,
          //   children: [
          //     TextButton(onPressed: () {}, child: Text('Overview')),
          //     TextButton(onPressed: () {}, child: Text('Students')),
          //     TextButton(onPressed: () {}, child: Text('Ratings')),
          //   ],
          // ),
          SizedBox(
            width: 400.0,
            child: Card
            (
              child: Padding(
                padding: const EdgeInsets.all(20.0),
                child: Column(
                  
                  children: [
                    
                    Text('Upcoming Trips'),
                    Text('Your scheduled routes for today and tomorrow'),
                  ],
                ),
              ),
            ),
          ),
          SizedBox(
            
            width: 400.0,
            child: Card(
              child: Padding(
                padding: const EdgeInsets.all(20.0),
                child: Column(
                  children: [
                    Text('Total Students'),
                    Text('12'),
                    Text('+2 from last week'),
                  ],
                ),
              ),
            ),
          ),
          SizedBox
          (
            width: 400.0,
           child: Card(
              child: Padding(
                padding: const EdgeInsets.all(20.0),
                child: Column(
                  children: [
                    Text('Average Rating'),
                    Text('4.8'),
                  ],
                ),
              ),
            ),
          ) , 
          SizedBox(
            width: 400.0,
            child:  Card(
                child: Padding(
                  padding: const EdgeInsets.all(20.0),
                  child: Column(
                    children: [
                      Text('Next Trip:'),
                      Text('7:30 AM'),
                      Text('Morning Route #42'),
                    ],
                  ),
                ),
              ),
            ),
          
        ],
      ),
    );
  }
}
