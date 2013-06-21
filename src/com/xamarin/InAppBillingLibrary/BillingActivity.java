package com.xamarin.InAppBillingLibrary;

import android.app.Activity;
import android.content.ComponentName;
import android.content.Intent;
import android.content.ServiceConnection;
import android.os.Bundle;
import android.os.IBinder;
import android.os.RemoteException;
import android.util.Log;
import com.android.vending.billing.IInAppBillingService;

/**
 * Created with IntelliJ IDEA.
 * User: Prashant Cholachagudda
 * Date: 21/06/13
 * Time: 12:40 PM
 */
public class BillingActivity extends Activity {

    final String TAG = "** IN_APP_BILLING";
    ServiceConnection mServiceConn;
    private IInAppBillingService mService;
    private int API_VERSION = 3;
    private boolean isSubscriptionSupported = false;
    private boolean isInAppBillingSupported = true;


    /**
     * Starts the setup process. This method will starts up the connection process
     * and notified through listener.
     * @param listener The listener to notify when the setup is complete
     */
    protected void setupBillingServiceConnection(final OnServiceCreatedListener listener){
       mServiceConn = new ServiceConnection() {
            @Override
            public void onServiceConnected(ComponentName componentName, IBinder service) {

                Log.d(TAG, "Service connected");

                mService = IInAppBillingService.Stub.asInterface(service);
                try {
                    int supported = mService.isBillingSupported(API_VERSION, getPackageName(), "inapp");
                    if (supported != 0) {
                        setIsSubscriptionSupported(false);
                    }

                    supported = mService.isBillingSupported(API_VERSION, getPackageName(), "subs");
                    if (supported == 0) {
                        setIsSubscriptionSupported(true);
                    }

                    listener.onServiceCreationFinished(true);

                } catch (RemoteException e) {
                    isInAppBillingSupported = false;
                    listener.onServiceCreationFinished(false);
                }
            }

            @Override
            public void onServiceDisconnected(ComponentName componentName) {
                mService = null;
            }
        };

    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        bindService(new
                Intent("com.android.vending.billing.InAppBillingService.BIND"),
                mServiceConn, BIND_AUTO_CREATE);
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        if (mServiceConn != null) {
            unbindService(mServiceConn);
        }
    }

    protected boolean getIsSubscriptionSupported() {
        return isSubscriptionSupported;
    }

    protected void setIsSubscriptionSupported(boolean value) {
        isSubscriptionSupported = value;
    }

    protected boolean getIsInAppBillingSupported() {
        return isInAppBillingSupported;
    }

    protected IInAppBillingService getBillingService() {
        return mService;
    }

    /**
     * Service creation listener
     */
    public interface OnServiceCreatedListener {
        /**
         * Notification method
         * @param isServiceCreated true if service is initialised; false otherwise
         */
          public void onServiceCreationFinished(boolean isServiceCreated);
    }
}
