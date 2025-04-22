import 'package:flutter/material.dart';

class BehaviorReportsPage extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        backgroundColor: Colors.white,
        title: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Recent Behavior Reports',
              style: TextStyle(
                  fontSize: 20,
                  fontWeight: FontWeight.bold,
                  color: Colors.black),
            ),
            Text(
              'Student behavior observations',
              style: TextStyle(fontSize: 14, color: Colors.grey),
            ),
          ],
        ),
      ),
      body: Padding(
        padding: const EdgeInsets.all(10.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            SizedBox(
              width: double.infinity,
              child: _buildStudentCard(
                student: 'Emma Johnosn',
                date: 'Mar 17, 2025',
                type: 'Positive',
                description: 'Helped a younger...',
                status: 'Notified',
              ),
            ),
            SizedBox(height: 16),
            SizedBox(
              width: double.infinity,
              child: _buildStudentCard(
                student: 'Noah Williams',
                date: 'Mar 10, 2025',
                type: 'Negative',
                description: 'Excessive noise ',
                status: 'Pending',
              ),
            ),
            SizedBox(height: 16),
            SizedBox(
              width: double.infinity,
              child: _buildStudentCard(
                student: 'Olivia Brown',
                date: 'Mar 16, 2025',
                type: 'Positive',
                description: 'Excellent behavior...',
                status: 'Notified',
              ),
            ),
            SizedBox(height: 16),
            SizedBox(
              width: double.infinity,
              child: _buildStudentCard(
                student: 'Liam Davis',
                date: 'Mar 18, 2025',
                type: 'Negative',
                description: 'Punished his friend',
                status: 'Pending',
              ),
            ),
            SizedBox(height: 20),
            ElevatedButton(
              style: ElevatedButton.styleFrom(
                backgroundColor: Colors.black,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(20),
                ),
              ),
              onPressed: () {},
              child: Text(
                'New Behavior Report',
                style: TextStyle(color: Colors.white),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildStudentCard({
    required String student,
    required String date,
    required String type,
    required String description,
    required String status,
  }) {
    return Card(
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(10),
      ),
      child: Padding(
        padding: const EdgeInsets.all(24.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Student: $student',
              style: TextStyle(fontSize: 20),
            ),
            Text(
              'Date: $date',
              style: TextStyle(fontSize: 20),
            ),
            Text(
              'Type: $type',
              style: TextStyle(fontSize: 20),
            ),
            Text(
              'Description: $description',
              style: TextStyle(fontSize: 20),
            ),
            Text(
              'Status: $status',
              style: TextStyle(fontSize: 20),
            ),
          ],
        ),
      ),
    );
  }
}
