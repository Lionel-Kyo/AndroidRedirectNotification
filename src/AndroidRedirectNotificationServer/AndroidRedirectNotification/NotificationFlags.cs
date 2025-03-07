using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidRedirectNotification
{
    internal enum NotificationFlags
    {
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     Portions of this page are modifications based on work created and shared by the
        //     Android Open Source Project and used according to terms described in the Creative
        //     Commons 2.5 Attribution License.
        ShowLights = 1,
        //
        // Summary:
        //     Bit to be bitwise-ored into the Android.App.Notification.Flagsfield that should
        //     be set if this notification is in reference to something that is ongoing, like
        //     a phone call.
        //
        // Remarks:
        //     Portions of this page are modifications based on work created and shared by the
        //     Android Open Source Project and used according to terms described in the Creative
        //     Commons 2.5 Attribution License.
        OngoingEvent = 2,
        //
        // Summary:
        //     Bit to be bitwise-ored into the Android.App.Notification.Flagsfield that if set,
        //     the audio will be repeated until the notification is cancelled or the notification
        //     window is opened.
        //
        // Remarks:
        //     Portions of this page are modifications based on work created and shared by the
        //     Android Open Source Project and used according to terms described in the Creative
        //     Commons 2.5 Attribution License.
        Insistent = 4,
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     Portions of this page are modifications based on work created and shared by the
        //     Android Open Source Project and used according to terms described in the Creative
        //     Commons 2.5 Attribution License.
        OnlyAlertOnce = 8,
        //
        // Summary:
        //     Bit to be bitwise-ored into the Android.App.Notification.Flagsfield that should
        //     be set if the notification should be canceled when it is clicked by the user.
        //
        //
        // Remarks:
        //     Portions of this page are modifications based on work created and shared by the
        //     Android Open Source Project and used according to terms described in the Creative
        //     Commons 2.5 Attribution License.
        AutoCancel = 16,
        //
        // Summary:
        //     Bit to be bitwise-ored into the Android.App.Notification.Flagsfield that should
        //     be set if the notification should not be canceled when the user clicks the Clear
        //     all button.
        //
        // Remarks:
        //     Portions of this page are modifications based on work created and shared by the
        //     Android Open Source Project and used according to terms described in the Creative
        //     Commons 2.5 Attribution License.
        NoClear = 32,
        //
        // Summary:
        //     Bit to be bitwise-ored into the Android.App.Notification.Flagsfield that should
        //     be set if this notification represents a currently running service.
        //
        // Remarks:
        //     Portions of this page are modifications based on work created and shared by the
        //     Android Open Source Project and used according to terms described in the Creative
        //     Commons 2.5 Attribution License.
        ForegroundService = 64,
        //
        // Summary:
        //     Obsolete flag indicating high-priority notifications; use the priority field
        //     instead.
        //
        // Remarks:
        //     Portions of this page are modifications based on work created and shared by the
        //     Android Open Source Project and used according to terms described in the Creative
        //     Commons 2.5 Attribution License.
        HighPriority = 128,
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     Portions of this page are modifications based on work created and shared by the
        //     Android Open Source Project and used according to terms described in the Creative
        //     Commons 2.5 Attribution License.
        LocalOnly = 256,
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     Portions of this page are modifications based on work created and shared by the
        //     Android Open Source Project and used according to terms described in the Creative
        //     Commons 2.5 Attribution License.
        GroupSummary = 512,
        //
        // Summary:
        //     To be added.
        Bubble = 4096
    }
}
