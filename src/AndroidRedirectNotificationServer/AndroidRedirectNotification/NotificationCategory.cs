using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidRedirectNotification
{
    internal static class NotificationCategory
    {
        // Summary:
        //     Notification category: system or device status update.
        //
        // Remarks:
        //     Java documentation for
        //
        //     android.app.Notification.CATEGORY_SYSTEM
        //
        //     .
        //
        //     Portions of this page are modifications based on work created and shared by the
        //     Android Open Source Project and used according to terms described in the Creative
        //     Commons 2.5 Attribution License.
        public const string CategorySystem = "sys";
        //
        // Summary:
        //     Notification category: running stopwatch.
        //
        // Remarks:
        //     Java documentation for
        //
        //     android.app.Notification.CATEGORY_STOPWATCH
        //
        //     .
        //
        //     Portions of this page are modifications based on work created and shared by the
        //     Android Open Source Project and used according to terms described in the Creative
        //     Commons 2.5 Attribution License.
        public const string CategoryStopwatch = "stopwatch";
        //
        // Summary:
        //     Notification category: ongoing information about device or contextual status.
        //
        //
        // Remarks:
        //     Java documentation for
        //
        //     android.app.Notification.CATEGORY_STATUS
        //
        //     .
        //
        //     Portions of this page are modifications based on work created and shared by the
        //     Android Open Source Project and used according to terms described in the Creative
        //     Commons 2.5 Attribution License.
        public const string CategoryStatus = "status";
        //
        // Summary:
        //     Notification category: social network or sharing update.
        //
        // Remarks:
        //     Java documentation for
        //
        //     android.app.Notification.CATEGORY_SOCIAL
        //
        //     .
        //
        //     Portions of this page are modifications based on work created and shared by the
        //     Android Open Source Project and used according to terms described in the Creative
        //     Commons 2.5 Attribution License.
        public const string CategorySocial = "social";
        //
        // Summary:
        //     Notification category: indication of running background service.
        //
        // Remarks:
        //     Java documentation for
        //
        //     android.app.Notification.CATEGORY_SERVICE
        //
        //     .
        //
        //     Portions of this page are modifications based on work created and shared by the
        //     Android Open Source Project and used according to terms described in the Creative
        //     Commons 2.5 Attribution License.
        public const string CategoryService = "service";
        //
        // Summary:
        //     Notification category: user-scheduled reminder.
        //
        // Remarks:
        //     Java documentation for
        //
        //     android.app.Notification.CATEGORY_REMINDER
        //
        //     .
        //
        //     Portions of this page are modifications based on work created and shared by the
        //     Android Open Source Project and used according to terms described in the Creative
        //     Commons 2.5 Attribution License.
        public const string CategoryReminder = "reminder";
        //
        // Summary:
        //     Notification category: a specific, timely recommendation for a single thing.
        //
        //
        // Remarks:
        //     Java documentation for
        //
        //     android.app.Notification.CATEGORY_RECOMMENDATION
        //
        //     .
        //
        //     Portions of this page are modifications based on work created and shared by the
        //     Android Open Source Project and used according to terms described in the Creative
        //     Commons 2.5 Attribution License.
        public const string CategoryRecommendation = "recommendation";
        //
        // Summary:
        //     Notification category: promotion or advertisement.
        //
        // Remarks:
        //     Java documentation for
        //
        //     android.app.Notification.CATEGORY_PROMO
        //
        //     .
        //
        //     Portions of this page are modifications based on work created and shared by the
        //     Android Open Source Project and used according to terms described in the Creative
        //     Commons 2.5 Attribution License.
        public const string CategoryPromo = "promo";
        //
        // Summary:
        //     Notification category: progress of a long-running background operation.
        //
        // Remarks:
        //     Java documentation for
        //
        //     android.app.Notification.CATEGORY_PROGRESS
        //
        //     .
        //
        //     Portions of this page are modifications based on work created and shared by the
        //     Android Open Source Project and used according to terms described in the Creative
        //     Commons 2.5 Attribution License.
        public const string CategoryProgress = "progress";
        //
        // Summary:
        //     Notification category: map turn-by-turn navigation.
        //
        // Remarks:
        //     Java documentation for
        //
        //     android.app.Notification.CATEGORY_NAVIGATION
        //
        //     .
        //
        //     Portions of this page are modifications based on work created and shared by the
        //     Android Open Source Project and used according to terms described in the Creative
        //     Commons 2.5 Attribution License.
        public const string CategoryNavigation = "navigation";
        //
        // Summary:
        //     Notification category: missed call.
        //
        // Remarks:
        //     Java documentation for
        //
        //     android.app.Notification.CATEGORY_MISSED_CALL
        //
        //     .
        //
        //     Portions of this page are modifications based on work created and shared by the
        //     Android Open Source Project and used according to terms described in the Creative
        //     Commons 2.5 Attribution License.
        public const string CategoryMissedCall = "missed_call";
        //
        // Summary:
        //     Notification category: incoming direct message (SMS, instant message, etc.
        //
        // Remarks:
        //     Java documentation for
        //
        //     android.app.Notification.CATEGORY_MESSAGE
        //
        //     .
        //
        //     Portions of this page are modifications based on work created and shared by the
        //     Android Open Source Project and used according to terms described in the Creative
        //     Commons 2.5 Attribution License.
        public const string CategoryMessage = "msg";
        //
        // Summary:
        //     Notification category: temporarily sharing location.
        //
        // Remarks:
        //     Java documentation for
        //
        //     android.app.Notification.CATEGORY_LOCATION_SHARING
        //
        //     .
        //
        //     Portions of this page are modifications based on work created and shared by the
        //     Android Open Source Project and used according to terms described in the Creative
        //     Commons 2.5 Attribution License.
        public const string CategoryLocationSharing = "location_sharing";
        //
        // Summary:
        //     Notification category: calendar event.
        //
        // Remarks:
        //     Java documentation for
        //
        //     android.app.Notification.CATEGORY_EVENT
        //
        //     .
        //
        //     Portions of this page are modifications based on work created and shared by the
        //     Android Open Source Project and used according to terms described in the Creative
        //     Commons 2.5 Attribution License.
        public const string CategoryEvent = "event";
        //
        // Summary:
        //     Notification category: error in background operation or authentication status.
        //
        //
        // Remarks:
        //     Java documentation for
        //
        //     android.app.Notification.CATEGORY_ERROR
        //
        //     .
        //
        //     Portions of this page are modifications based on work created and shared by the
        //     Android Open Source Project and used according to terms described in the Creative
        //     Commons 2.5 Attribution License.
        public const string CategoryError = "err";
        //
        // Summary:
        //     Notification category: incoming call (voice or video) or similar synchronous
        //     communication request.
        //
        // Remarks:
        //     Java documentation for
        //
        //     android.app.Notification.CATEGORY_CALL
        //
        //     .
        //
        //     Portions of this page are modifications based on work created and shared by the
        //     Android Open Source Project and used according to terms described in the Creative
        //     Commons 2.5 Attribution License.
        public const string CategoryCall = "call";
        //
        // Summary:
        //     #extras key: whether the android.app.Notification.MessagingStyle notification
        //     represents a group conversation.
        //
        // Remarks:
        //     Java documentation for
        //
        //     android.app.Notification.EXTRA_IS_GROUP_CONVERSATION
        //
        //     .
        //
        //     Portions of this page are modifications based on work created and shared by the
        //     Android Open Source Project and used according to terms described in the Creative
        //     Commons 2.5 Attribution License.
        public const string ExtraIsGroupConversation = "android.isGroupConversation";
        //
        // Summary:
        //     Notification category: media transport control for playback.
        //
        // Remarks:
        //     Java documentation for
        //
        //     android.app.Notification.CATEGORY_TRANSPORT
        //
        //     .
        //
        //     Portions of this page are modifications based on work created and shared by the
        //     Android Open Source Project and used according to terms described in the Creative
        //     Commons 2.5 Attribution License.
        public const string CategoryTransport = "transport";
        //
        // Summary:
        //     Notification category: tracking a user's workout.
        //
        // Remarks:
        //     Java documentation for
        //
        //     android.app.Notification.CATEGORY_WORKOUT
        //
        //     .
        //
        //     Portions of this page are modifications based on work created and shared by the
        //     Android Open Source Project and used according to terms described in the Creative
        //     Commons 2.5 Attribution License.
        public const string CategoryWorkout = "workout";
        //
        // Summary:
        //     Notification category: asynchronous bulk message (email).
        //
        // Remarks:
        //     Java documentation for
        //
        //     android.app.Notification.CATEGORY_EMAIL
        //
        //     .
        //
        //     Portions of this page are modifications based on work created and shared by the
        //     Android Open Source Project and used according to terms described in the Creative
        //     Commons 2.5 Attribution License.
        public const string CategoryEmail = "email";
    }
}
