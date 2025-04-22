import 'package:flutter/material.dart';

class StudentStatus extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        backgroundColor: Colors.white,
        title: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Student Status',
              style: TextStyle(
                  fontSize: 20,
                  fontWeight: FontWeight.bold,
                  color: Colors.black),
            ),
            Text(
              'Current attendance status for all students',
              style: TextStyle(fontSize: 14, color: Colors.grey),
            ),
          ],
        ),
      ),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            
            
            SizedBox(height: 20),
            Column(
              children: [
                SizedBox(
                  width: double.infinity,
                  child: _buildStudentCard(
                    student: 'Emma Johnson',
                    grade: '5th',
                    route: 'Route A',
                    status: 'On Board',
                  ),
                ),
                SizedBox(height: 16),
                SizedBox(
                  width: double.infinity,
                  child: _buildStudentCard(
                    student: 'Noah Williams',
                    grade: '3rd',
                    route: 'Route B',
                    status: 'On Board',
                  ),
                ),
                SizedBox(height: 16),
                SizedBox(
                  width: double.infinity,
                  child: _buildStudentCard(
                    student: 'Olivia Brown',
                    grade: '4th',
                    route: 'Route A',
                    status: 'On Board',
                  ),
                ),
                SizedBox(height: 16),
                SizedBox(
                  width: double.infinity,
                  child: _buildStudentCard(
                    student: 'Liam Davis',
                    grade: '2nd',
                    route: 'Route C',
                    status: 'Absent',
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

 Widget _buildStudentCard({
    required String student,
    required String grade,
    required String route,
    required String status,
  }) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Student: $student',
              style: TextStyle(fontSize: 18),
            ),
            Text(
              'Grade: $grade',
              style: TextStyle(fontSize: 18),
            ),
            Text(
              'Route: $route',
              style: TextStyle(fontSize: 18),
            ),
            Text(
              'Status: $status',
              style: TextStyle(fontSize: 18),
            ),
          ],
        ),
      ),
    );
  }
}
