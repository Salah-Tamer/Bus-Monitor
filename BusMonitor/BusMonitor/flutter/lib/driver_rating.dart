import 'package:flutter/material.dart';

class DriverRating extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('Your Ratings'),
      ),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Your Ratings',
              style: TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
            ),
            Text(
              'Feedback from supervisors and parents',
              style: TextStyle(fontSize: 16, color: Colors.grey),
            ),
            SizedBox(height: 16),
            RatingCard(
              reviewer: 'Jane Smith (Supervisor)',
              rating: 5,
              feedback: 'Excellent driving and punctuality',
              date: 'Yesterday',
            ),
            RatingCard(
              reviewer: 'Parent of Emma Thompson',
              rating: 4,
              feedback: 'Very reliable and friendly',
              date: 'Last week',
            ),
            RatingCard(
              reviewer: 'School Administrator',
              rating: 4,
              feedback: 'Great communication and safety practices',
              date: '2 weeks ago',
            ),
            SizedBox(height: 16),
            Text(
              'Average Rating',
              style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
            ),
            SizedBox(height: 8),
            Row(
              children: [
                Text('4.7/5.0'),
                SizedBox(width: 16),
                Expanded(
                  child: LinearProgressIndicator(
                    value: 0.94,
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

class RatingCard extends StatelessWidget {
  final String reviewer;
  final int rating;
  final String feedback;
  final String date;

  RatingCard({
    required this.reviewer,
    required this.rating,
    required this.feedback,
    required this.date,
  });

  @override
  Widget build(BuildContext context) {
    return Card(
      elevation: 2,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(8),
        side: BorderSide(
          color: Colors.blue,
          width: 1,
        ),
      ),
      margin: EdgeInsets.symmetric(vertical: 8),
      child: Padding(
        padding: const EdgeInsets.all(12.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(reviewer, style: TextStyle(fontWeight: FontWeight.bold)),
            Row(
              children: List.generate(5, (index) {
                return Icon(
                  index < rating ? Icons.star : Icons.star_border,
                  color: Colors.amber,
                  size: 16,
                );
              }),
            ),
            Text(feedback),
            SizedBox(height: 4),
            Align(
              alignment: Alignment.bottomRight,
              child: Text(date, style: TextStyle(fontSize: 12, color: Colors.grey)),
            ),
          ],
        ),
      ),
    );
  }
}
